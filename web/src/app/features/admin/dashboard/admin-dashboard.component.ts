import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Chart, ChartConfiguration } from 'chart.js/auto';
import { forkJoin } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';

interface DashboardTile {
  title: string;
  value: string;
  linkText: string;
  className: string;
  icon: string;
}

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="row gx-4 gy-4 mb-4">
      <div class="col-12 col-md-6 col-xl-3" *ngFor="let tile of summaryTiles">
        <div class="card text-white h-100 kpi-card" [ngClass]="tile.className">
          <div class="card-body d-flex justify-content-between align-items-start">
            <div>
              <div class="small">{{ tile.title }}</div>
              <div class="h2 mb-0">{{ tile.value }}</div>
            </div>
            <i class="bi tile-icon" [ngClass]="tile.icon"></i>
          </div>
          <a class="card-footer d-flex align-items-center justify-content-between small text-white text-decoration-none" href="javascript:void(0)">
            <span>{{ tile.linkText }}</span>
            <i class="bi bi-chevron-right"></i>
          </a>
        </div>
      </div>
    </div>

    <div class="row gx-4 gy-4 mb-4">
      <div class="col-12 col-xl-6">
        <div class="card h-100">
          <div class="card-header fw-600 text-primary d-flex justify-content-between align-items-center">
            Earnings Breakdown
            <i class="bi bi-three-dots-vertical text-muted"></i>
          </div>
          <div class="card-body chart-card-body">
            <canvas #earningsCanvas aria-label="Earnings breakdown chart"></canvas>
          </div>
        </div>
      </div>

      <div class="col-12 col-xl-6">
        <div class="card h-100">
          <div class="card-header fw-600 text-primary d-flex justify-content-between align-items-center">
            Monthly Revenue
            <i class="bi bi-three-dots-vertical text-muted"></i>
          </div>
          <div class="card-body chart-card-body">
            <canvas #revenueCanvas aria-label="Monthly revenue chart"></canvas>
          </div>
        </div>
      </div>
    </div>

    <div class="card mb-4">
      <div class="card-header">Personnel Management</div>
      <div class="card-body">
        <div class="row g-3 align-items-center mb-3">
          <div class="col-12 col-md-6 d-flex align-items-center gap-2">
            <select class="form-select w-auto" [(ngModel)]="pageSize" aria-label="Entries per page">
              <option [ngValue]="10">10</option>
              <option [ngValue]="25">25</option>
              <option [ngValue]="50">50</option>
            </select>
            <span class="text-muted">entries per page</span>
          </div>
          <div class="col-12 col-md-4 ms-md-auto">
            <input class="form-control" [(ngModel)]="searchTerm" placeholder="Search..." aria-label="Search orders" />
          </div>
        </div>

        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <th>Order</th>
                <th>Customer</th>
                <th>Status</th>
                <th>Total</th>
                <th>Date</th>
                <th>Payment</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let order of filteredOrders">
                <td class="fw-semibold">{{ order.orderNumber }}</td>
                <td>{{ order.customerName }}</td>
                <td>
                  <span class="badge rounded-pill" [ngClass]="statusBadgeClass(order.status)">{{ order.status }}</span>
                </td>
                <td>{{ currency(order.totalAmount || 0) }}</td>
                <td>{{ (order.createdAtUtc || order.createdAt) | date:'mediumDate' }}</td>
                <td>
                  <span class="badge rounded-pill" [ngClass]="paymentBadgeClass(order)">{{ paymentLabel(order) }}</span>
                </td>
              </tr>
              <tr *ngIf="!filteredOrders.length">
                <td colspan="6" class="text-muted">No orders found.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: [
    '.kpi-card .card-footer{background:rgba(0,0,0,.12);}',
    '.kpi-primary{background:#0061f2;}.kpi-warning{background:#f4a100;}.kpi-success{background:#00ac69;}.kpi-danger{background:#e81500;}',
    '.tile-icon{font-size:2.25rem;opacity:.55;}',
    '.chart-card-body{height:320px;}',
    '@media (max-width: 1199.98px){.chart-card-body{height:280px;}}'
  ]
})
export class AdminDashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('earningsCanvas') earningsCanvas?: ElementRef<HTMLCanvasElement>;
  @ViewChild('revenueCanvas') revenueCanvas?: ElementRef<HTMLCanvasElement>;

  report: any = {};
  orders: any[] = [];

  searchTerm = '';
  pageSize = 10;

  summaryTiles: DashboardTile[] = [];

  private lineChart?: Chart;
  private barChart?: Chart;
  private viewReady = false;

  constructor(private readonly api: ApiService) {}

  get filteredOrders(): any[] {
    const query = this.searchTerm.trim().toLowerCase();
    const rows = !query
      ? this.orders
      : this.orders.filter(
          o =>
            String(o.orderNumber || '')
              .toLowerCase()
              .includes(query) ||
            String(o.customerName || '')
              .toLowerCase()
              .includes(query) ||
            String(o.status || '')
              .toLowerCase()
              .includes(query)
        );

    return rows.slice(0, this.pageSize);
  }

  ngOnInit(): void {
    this.loadReport();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
    this.renderCharts();
  }

  ngOnDestroy(): void {
    this.destroyCharts();
  }

  loadReport(): void {
    const now = new Date();
    const from = new Date(now.getFullYear(), now.getMonth(), 1);

    forkJoin({
      report: this.api.adminSalesReport(this.toIsoDateStart(from), this.toIsoDateEnd(now)),
      orders: this.api.adminOrders()
    }).subscribe({
      next: ({ report, orders }) => {
        this.report = report || {};
        this.orders = Array.isArray(orders) ? orders : [];
        this.orders.sort((a, b) => (this.orderDate(b)?.getTime() || 0) - (this.orderDate(a)?.getTime() || 0));
        this.buildSummaryTiles();
        this.renderCharts();
      },
      error: () => {
        this.report = {};
        this.orders = [];
        this.summaryTiles = [];
        this.renderCharts();
      }
    });
  }

  statusBadgeClass(status: string): string {
    const normalized = String(status || '').toLowerCase();
    if (normalized.includes('deliver')) return 'bg-success text-white';
    if (normalized.includes('cancel') || normalized.includes('refund')) return 'bg-danger text-white';
    if (normalized.includes('out')) return 'bg-info text-white';
    if (normalized.includes('prepar')) return 'bg-warning text-dark';
    return 'bg-primary text-white';
  }

  paymentBadgeClass(order: any): string {
    const label = this.paymentLabel(order).toLowerCase();
    return label === 'paid' ? 'bg-primary text-white' : 'bg-secondary text-white';
  }

  paymentLabel(order: any): string {
    return order?.isPaid ? 'Paid' : order?.paymentMethod || 'Pending';
  }

  currency(value: number): string {
    return `AED ${Math.round(Number(value) || 0)}`;
  }

  private buildSummaryTiles(): void {
    const monthStart = new Date();
    monthStart.setDate(1);
    monthStart.setHours(0, 0, 0, 0);

    const yearStart = new Date(new Date().getFullYear(), 0, 1);

    const monthly = this.orders
      .filter(o => {
        const date = this.orderDate(o);
        return date && date >= monthStart;
      })
      .reduce((sum, o) => sum + this.orderAmount(o), 0);

    const annual = this.orders
      .filter(o => {
        const date = this.orderDate(o);
        return date && date >= yearStart;
      })
      .reduce((sum, o) => sum + this.orderAmount(o), 0);

    const deliveredCount = this.orders.filter(o => String(o.status || '').toLowerCase().includes('deliver')).length;
    const pendingCount = this.orders.filter(o => ['pending', 'preparing', 'outfordelivery', 'out for delivery'].includes(String(o.status || '').toLowerCase())).length;

    this.summaryTiles = [
      { title: 'Earnings (Monthly)', value: this.currency(monthly), linkText: 'View Report', className: 'kpi-primary', icon: 'bi-calendar2-week' },
      { title: 'Earnings (Annual)', value: this.currency(annual || this.report.paidSales || 0), linkText: 'View Report', className: 'kpi-warning', icon: 'bi-currency-dollar' },
      { title: 'Task Completion', value: String(deliveredCount), linkText: 'View Tasks', className: 'kpi-success', icon: 'bi-check2-square' },
      { title: 'Pending Requests', value: String(pendingCount), linkText: 'View Requests', className: 'kpi-danger', icon: 'bi-chat-left-text' }
    ];
  }

  private renderCharts(): void {
    if (!this.viewReady || !this.earningsCanvas || !this.revenueCanvas) {
      return;
    }

    this.destroyCharts();

    const monthlyTotals = this.aggregateByMonth(12);
    const recentTotals = this.aggregateByMonth(6);

    const lineConfig: ChartConfiguration<'line'> = {
      type: 'line',
      data: {
        labels: monthlyTotals.labels,
        datasets: [
          {
            label: 'Revenue',
            data: monthlyTotals.values,
            tension: 0.35,
            borderWidth: 3,
            borderColor: '#0061f2',
            pointRadius: 4,
            pointHoverRadius: 5,
            pointBackgroundColor: '#0061f2',
            fill: true,
            backgroundColor: 'rgba(0, 97, 242, 0.1)'
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true, ticks: { callback: value => `AED ${value}` } } }
      }
    };

    const barConfig: ChartConfiguration<'bar'> = {
      type: 'bar',
      data: {
        labels: recentTotals.labels,
        datasets: [
          {
            data: recentTotals.values,
            borderRadius: 6,
            backgroundColor: '#0061f2',
            maxBarThickness: 28
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true, ticks: { callback: value => `AED ${value}` } } }
      }
    };

    this.lineChart = new Chart(this.earningsCanvas.nativeElement, lineConfig);
    this.barChart = new Chart(this.revenueCanvas.nativeElement, barConfig);
  }

  private destroyCharts(): void {
    this.lineChart?.destroy();
    this.barChart?.destroy();
  }

  private aggregateByMonth(months: number): { labels: string[]; values: number[] } {
    const monthStarts: Date[] = [];
    const labels: string[] = [];

    const now = new Date();
    for (let i = months - 1; i >= 0; i--) {
      const d = new Date(now.getFullYear(), now.getMonth() - i, 1);
      monthStarts.push(d);
      labels.push(d.toLocaleString('en-US', { month: 'short' }));
    }

    const values = monthStarts.map(start => {
      const end = new Date(start.getFullYear(), start.getMonth() + 1, 1);
      return this.orders
        .filter(o => {
          const date = this.orderDate(o);
          return date && date >= start && date < end;
        })
        .reduce((sum, o) => sum + this.orderAmount(o), 0);
    });

    return { labels, values };
  }

  private orderDate(order: any): Date | null {
    const raw = order?.createdAtUtc || order?.createdAt || order?.placedAtUtc || order?.updatedAtUtc;
    if (!raw) {
      return null;
    }

    const d = new Date(raw);
    return Number.isNaN(d.getTime()) ? null : d;
  }

  private orderAmount(order: any): number {
    const raw = order?.totalAmount ?? order?.grandTotal ?? order?.total;
    const value = Number(raw);
    return Number.isFinite(value) ? value : 0;
  }

  private toIsoDateStart(date: Date): string {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), 0, 0, 0).toISOString();
  }

  private toIsoDateEnd(date: Date): string {
    return new Date(date.getFullYear(), date.getMonth(), date.getDate(), 23, 59, 59).toISOString();
  }
}

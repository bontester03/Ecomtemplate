import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApiService } from '../../../core/services/api.service';

@Component({
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <div>
        <h1 class="mb-1">Delivery Rules</h1>
        <p class="text-muted mb-0">Configure zones, slots, and same-day cutoff.</p>
      </div>
    </div>

    <div class="card mb-4">
      <div class="card-header">Same-Day Cutoff</div>
      <div class="card-body">
        <div class="row g-3 align-items-end">
          <div class="col-12 col-md-4">
            <label class="form-label">Cutoff Hour (0-23)</label>
            <input class="form-control" type="number" min="0" max="23" [(ngModel)]="sameDayCutoffHour" />
          </div>
          <div class="col-12 col-md-3">
            <button class="btn btn-primary w-100" type="button" (click)="saveCutoff()">Save Cutoff</button>
          </div>
        </div>
      </div>
    </div>

    <div class="card mb-4">
      <div class="card-header">Delivery Zones</div>
      <div class="card-body border-bottom">
        <div class="row g-3 align-items-end">
          <div class="col-12 col-md-3">
            <label class="form-label">Emirate</label>
            <input class="form-control" [(ngModel)]="zoneForm.emirate" />
          </div>
          <div class="col-12 col-md-3">
            <label class="form-label">Area</label>
            <input class="form-control" [(ngModel)]="zoneForm.area" />
          </div>
          <div class="col-12 col-md-2">
            <label class="form-label">Charge</label>
            <input class="form-control" type="number" min="0" [(ngModel)]="zoneForm.charge" />
          </div>
          <div class="col-12 col-md-2">
            <button class="btn btn-primary w-100" type="button" (click)="saveZone()">{{ zoneForm.id ? 'Update' : 'Add' }}</button>
          </div>
          <div class="col-12 col-md-2" *ngIf="zoneForm.id">
            <button class="btn btn-outline-secondary w-100" type="button" (click)="resetZoneForm()">Cancel</button>
          </div>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <th class="ps-4">Emirate</th>
                <th>Area</th>
                <th>Charge</th>
                <th>Status</th>
                <th class="text-end pe-4">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let zone of zones">
                <td class="ps-4">{{ zone.emirate }}</td>
                <td>{{ zone.area }}</td>
                <td>AED {{ zone.charge }}</td>
                <td>{{ zone.isActive ? 'Active' : 'Inactive' }}</td>
                <td class="text-end pe-4">
                  <div class="btn-group btn-group-sm" role="group" aria-label="Zone actions">
                    <button class="btn btn-outline-secondary" type="button" (click)="editZone(zone)">Edit</button>
                    <button class="btn btn-outline-danger" type="button" (click)="deleteZone(zone)">Delete</button>
                  </div>
                </td>
              </tr>
              <tr *ngIf="!zones.length">
                <td class="ps-4 text-muted" colspan="5">No zones configured.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>

    <div class="card mb-4">
      <div class="card-header">Delivery Time Slots</div>
      <div class="card-body border-bottom">
        <div class="row g-3 align-items-end">
          <div class="col-12 col-md-3">
            <label class="form-label">Label</label>
            <input class="form-control" [(ngModel)]="slotForm.label" />
          </div>
          <div class="col-6 col-md-2">
            <label class="form-label">Start</label>
            <input class="form-control" type="time" [(ngModel)]="slotForm.startTime" />
          </div>
          <div class="col-6 col-md-2">
            <label class="form-label">End</label>
            <input class="form-control" type="time" [(ngModel)]="slotForm.endTime" />
          </div>
          <div class="col-12 col-md-2">
            <label class="form-label">Capacity</label>
            <input class="form-control" type="number" min="1" [(ngModel)]="slotForm.capacityLimit" placeholder="Unlimited" />
          </div>
          <div class="col-6 col-md-1">
            <div class="form-check mt-4 pt-1">
              <input class="form-check-input" type="checkbox" id="slotActive" [(ngModel)]="slotForm.isActive" />
              <label class="form-check-label" for="slotActive">Active</label>
            </div>
          </div>
          <div class="col-6 col-md-2">
            <button class="btn btn-primary w-100" type="button" (click)="saveSlot()">{{ slotForm.id ? 'Update' : 'Add' }}</button>
          </div>
          <div class="col-12 col-md-2" *ngIf="slotForm.id">
            <button class="btn btn-outline-secondary w-100" type="button" (click)="resetSlotForm()">Cancel</button>
          </div>
        </div>
      </div>
      <div class="card-body p-0">
        <div class="table-responsive">
          <table class="table table-hover align-middle mb-0">
            <thead>
              <tr>
                <th class="ps-4">Label</th>
                <th>Start</th>
                <th>End</th>
                <th>Capacity</th>
                <th>Status</th>
                <th class="text-end pe-4">Actions</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let slot of slots">
                <td class="ps-4">{{ slot.label }}</td>
                <td>{{ slot.startTime }}</td>
                <td>{{ slot.endTime }}</td>
                <td>{{ slot.capacityLimit || 'Unlimited' }}</td>
                <td>{{ slot.isActive ? 'Active' : 'Inactive' }}</td>
                <td class="text-end pe-4">
                  <div class="btn-group btn-group-sm" role="group" aria-label="Slot actions">
                    <button class="btn btn-outline-secondary" type="button" (click)="editSlot(slot)">Edit</button>
                    <button class="btn btn-outline-danger" type="button" (click)="deleteSlot(slot)">Delete</button>
                  </div>
                </td>
              </tr>
              <tr *ngIf="!slots.length">
                <td class="ps-4 text-muted" colspan="6">No slots configured.</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `,
  styles: ['.btn-group .btn{min-width:70px;}']
})
export class DeliveryAdminComponent implements OnInit {
  zones: any[] = [];
  slots: any[] = [];
  sameDayCutoffHour = 14;

  zoneForm: any = { id: '', emirate: '', area: '', charge: 0, isActive: true };
  slotForm: any = { id: '', label: '', startTime: '10:00', endTime: '12:00', capacityLimit: null, isActive: true };

  constructor(private readonly api: ApiService) {}

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.api.adminZones().subscribe(data => (this.zones = data));
    this.api.adminSlots().subscribe(data => (this.slots = data));
    this.api.adminDeliverySettings().subscribe((s: any) => (this.sameDayCutoffHour = s.sameDayCutoffHour ?? 14));
  }

  saveCutoff(): void {
    this.api.adminUpdateDeliverySettings({ sameDayCutoffHour: Number(this.sameDayCutoffHour) }).subscribe();
  }

  editZone(zone: any): void {
    this.zoneForm = { ...zone };
  }

  resetZoneForm(): void {
    this.zoneForm = { id: '', emirate: '', area: '', charge: 0, isActive: true };
  }

  saveZone(): void {
    const payload = {
      emirate: this.zoneForm.emirate,
      area: this.zoneForm.area,
      charge: Number(this.zoneForm.charge),
      isActive: !!this.zoneForm.isActive
    };

    if (this.zoneForm.id) {
      this.api.adminUpdateZone(this.zoneForm.id, { id: this.zoneForm.id, ...payload }).subscribe(() => {
        this.resetZoneForm();
        this.loadAll();
      });
    } else {
      this.api.adminCreateZone(payload).subscribe(() => {
        this.resetZoneForm();
        this.loadAll();
      });
    }
  }

  deleteZone(zone: any): void {
    if (!confirm(`Delete zone ${zone.emirate} - ${zone.area}?`)) return;
    this.api.adminDeleteZone(zone.id).subscribe(() => this.loadAll());
  }

  editSlot(slot: any): void {
    this.slotForm = { ...slot };
  }

  resetSlotForm(): void {
    this.slotForm = { id: '', label: '', startTime: '10:00', endTime: '12:00', capacityLimit: null, isActive: true };
  }

  saveSlot(): void {
    const payload = {
      label: this.slotForm.label,
      startTime: this.slotForm.startTime,
      endTime: this.slotForm.endTime,
      capacityLimit: this.slotForm.capacityLimit ? Number(this.slotForm.capacityLimit) : null,
      isActive: !!this.slotForm.isActive
    };

    if (this.slotForm.id) {
      this.api.adminUpdateSlot(this.slotForm.id, { id: this.slotForm.id, ...payload }).subscribe(() => {
        this.resetSlotForm();
        this.loadAll();
      });
    } else {
      this.api.adminCreateSlot(payload).subscribe(() => {
        this.resetSlotForm();
        this.loadAll();
      });
    }
  }

  deleteSlot(slot: any): void {
    if (!confirm(`Delete slot ${slot.label}?`)) return;
    this.api.adminDeleteSlot(slot.id).subscribe(() => this.loadAll());
  }
}

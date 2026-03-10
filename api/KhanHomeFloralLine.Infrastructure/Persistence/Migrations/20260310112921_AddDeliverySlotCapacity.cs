using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhanHomeFloralLine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliverySlotCapacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CapacityLimit",
                table: "DeliveryTimeSlots",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AddOns",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444441"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8374));

            migrationBuilder.UpdateData(
                table: "AddOns",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444442"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8376));

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999991"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8627));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8109));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8111));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8113));

            migrationBuilder.UpdateData(
                table: "DeliveryTimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777771"),
                columns: new[] { "CapacityLimit", "CreatedAtUtc" },
                values: new object[] { 50, new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8482) });

            migrationBuilder.UpdateData(
                table: "DeliveryTimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777772"),
                columns: new[] { "CapacityLimit", "CreatedAtUtc" },
                values: new object[] { 50, new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8572) });

            migrationBuilder.UpdateData(
                table: "DeliveryTimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777773"),
                columns: new[] { "CapacityLimit", "CreatedAtUtc" },
                values: new object[] { 50, new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8574) });

            migrationBuilder.UpdateData(
                table: "DeliveryZones",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666661"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8453));

            migrationBuilder.UpdateData(
                table: "DeliveryZones",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666662"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8457));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8336));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8347));

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555551"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8398));

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555552"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8400));

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555553"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8402));

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555554"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8404));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333331"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8140));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333332"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8143));

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888881"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(8605));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 11, 29, 21, 496, DateTimeKind.Utc).AddTicks(7905));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CapacityLimit",
                table: "DeliveryTimeSlots");

            migrationBuilder.UpdateData(
                table: "AddOns",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444441"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8828));

            migrationBuilder.UpdateData(
                table: "AddOns",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444442"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8830));

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999991"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8988));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8740));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8742));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8744));

            migrationBuilder.UpdateData(
                table: "DeliveryTimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777771"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8939));

            migrationBuilder.UpdateData(
                table: "DeliveryTimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777772"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8941));

            migrationBuilder.UpdateData(
                table: "DeliveryTimeSlots",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777773"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8943));

            migrationBuilder.UpdateData(
                table: "DeliveryZones",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666661"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8908));

            migrationBuilder.UpdateData(
                table: "DeliveryZones",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666662"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8910));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8801));

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8806));

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555551"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8855));

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555552"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8858));

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555553"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8859));

            migrationBuilder.UpdateData(
                table: "ProductVariants",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555554"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8861));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333331"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8769));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333332"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8771));

            migrationBuilder.UpdateData(
                table: "PromoCodes",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888881"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8969));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAtUtc",
                value: new DateTime(2026, 3, 10, 10, 26, 3, 805, DateTimeKind.Utc).AddTicks(8596));
        }
    }
}

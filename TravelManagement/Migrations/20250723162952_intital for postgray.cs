﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TravelManagement.Migrations
{
    /// <inheritdoc />
    public partial class intitalforpostgray : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerName = table.Column<string>(type: "text", nullable: false),
                    CustomerNumber = table.Column<int>(type: "integer", nullable: false),
                    AlternateNumber = table.Column<int>(type: "integer", nullable: false),
                    TravelDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomersId);
                });

            migrationBuilder.CreateTable(
                name: "ExternalEmployees",
                columns: table => new
                {
                    externalEmployeeID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    externalEmployeeName = table.Column<string>(type: "text", nullable: true),
                    externalEmployeeNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalEmployees", x => x.externalEmployeeID);
                });

            migrationBuilder.CreateTable(
                name: "TravelAgents",
                columns: table => new
                {
                    AgentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    ContactNumber = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CommissionRate = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelAgents", x => x.AgentId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EmployeeName = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "text", nullable: false),
                    EmployeeDOB = table.Column<DateOnly>(type: "date", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Licence = table.Column<int>(type: "integer", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    Salary = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ResetPasswordtoken = table.Column<string>(type: "text", nullable: true),
                    RestPasswordExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RenewalMailSentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    EmployeAge = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleName = table.Column<string>(type: "text", nullable: true),
                    VehicleNumber = table.Column<string>(type: "text", nullable: true),
                    VehicleType = table.Column<int>(type: "integer", nullable: false),
                    RegistrationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    VehicleAge = table.Column<double>(type: "double precision", nullable: false),
                    Seatingcapacity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                });

            migrationBuilder.CreateTable(
                name: "salaries",
                columns: table => new
                {
                    SalaryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    BaseSalay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Deduction = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Overtimepay = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    NetSalaey = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    userID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salaries", x => x.SalaryId);
                    table.ForeignKey(
                        name: "FK_salaries_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    travelDate = table.Column<DateOnly>(type: "date", nullable: false),
                    From = table.Column<string>(type: "text", nullable: true),
                    To = table.Column<string>(type: "text", nullable: true),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    Userid = table.Column<int>(type: "integer", nullable: true),
                    BookingType = table.Column<int>(type: "integer", nullable: false),
                    Traveltime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Pax = table.Column<int>(type: "integer", nullable: false),
                    Assigned = table.Column<bool>(type: "boolean", nullable: false),
                    CustomerID = table.Column<int>(type: "integer", nullable: false),
                    ExternalEmployeeId = table.Column<int>(type: "integer", nullable: true),
                    Payment = table.Column<int>(type: "integer", nullable: false),
                    TravelAgentId = table.Column<int>(type: "integer", nullable: true),
                    Payments = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Customers_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomersId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bookings_ExternalEmployees_ExternalEmployeeId",
                        column: x => x.ExternalEmployeeId,
                        principalTable: "ExternalEmployees",
                        principalColumn: "externalEmployeeID");
                    table.ForeignKey(
                        name: "FK_Bookings_TravelAgents_TravelAgentId",
                        column: x => x.TravelAgentId,
                        principalTable: "TravelAgents",
                        principalColumn: "AgentId");
                    table.ForeignKey(
                        name: "FK_Bookings_Users_Userid",
                        column: x => x.Userid,
                        principalTable: "Users",
                        principalColumn: "userId");
                    table.ForeignKey(
                        name: "FK_Bookings_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    VehicleID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentID);
                    table.ForeignKey(
                        name: "FK_Documents_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicleExpences",
                columns: table => new
                {
                    VehicleExpenceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ExpenseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryType = table.Column<int>(type: "integer", nullable: false),
                    VehicleID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicleExpences", x => x.VehicleExpenceId);
                    table.ForeignKey(
                        name: "FK_vehicleExpences_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "vehicleMaintenanceShedules",
                columns: table => new
                {
                    VechicleMaintenanceSheduleId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServieDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Nextduedate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    cost = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    maintenanceType = table.Column<int>(type: "integer", nullable: false),
                    VehicleID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicleMaintenanceShedules", x => x.VechicleMaintenanceSheduleId);
                    table.ForeignKey(
                        name: "FK_vehicleMaintenanceShedules_Vehicles_VehicleID",
                        column: x => x.VehicleID,
                        principalTable: "Vehicles",
                        principalColumn: "VehicleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingPaymentAllocations",
                columns: table => new
                {
                    PaymentAllocationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BookingId = table.Column<int>(type: "integer", nullable: false),
                    PayerType = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: true),
                    CustomersId = table.Column<int>(type: "integer", nullable: true),
                    TravelAgentId = table.Column<int>(type: "integer", nullable: true),
                    AllocatedAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPaymentAllocations", x => x.PaymentAllocationId);
                    table.ForeignKey(
                        name: "FK_BookingPaymentAllocations_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingPaymentAllocations_Customers_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customers",
                        principalColumn: "CustomersId");
                    table.ForeignKey(
                        name: "FK_BookingPaymentAllocations_TravelAgents_TravelAgentId",
                        column: x => x.TravelAgentId,
                        principalTable: "TravelAgents",
                        principalColumn: "AgentId");
                });

            migrationBuilder.CreateTable(
                name: "overtimeLogs",
                columns: table => new
                {
                    OvertimeID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    hours = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    userId = table.Column<int>(type: "integer", nullable: false),
                    BookingId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_overtimeLogs", x => x.OvertimeID);
                    table.ForeignKey(
                        name: "FK_overtimeLogs_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_overtimeLogs_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AmountPaid = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentMethod = table.Column<string>(type: "text", nullable: true),
                    BookingId = table.Column<int>(type: "integer", nullable: false),
                    TravelAgentId = table.Column<int>(type: "integer", nullable: true),
                    CustomerId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomersId");
                    table.ForeignKey(
                        name: "FK_Payments_TravelAgents_TravelAgentId",
                        column: x => x.TravelAgentId,
                        principalTable: "TravelAgents",
                        principalColumn: "AgentId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingPaymentAllocations_BookingId",
                table: "BookingPaymentAllocations",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPaymentAllocations_CustomersId",
                table: "BookingPaymentAllocations",
                column: "CustomersId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPaymentAllocations_TravelAgentId",
                table: "BookingPaymentAllocations",
                column: "TravelAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_CustomerID",
                table: "Bookings",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ExternalEmployeeId",
                table: "Bookings",
                column: "ExternalEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TravelAgentId",
                table: "Bookings",
                column: "TravelAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Userid",
                table: "Bookings",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VehicleId",
                table: "Bookings",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_VehicleID",
                table: "Documents",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_overtimeLogs_BookingId",
                table: "overtimeLogs",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_overtimeLogs_userId",
                table: "overtimeLogs",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CustomerId",
                table: "Payments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_TravelAgentId",
                table: "Payments",
                column: "TravelAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_salaries_userID",
                table: "salaries",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicleExpences_VehicleID",
                table: "vehicleExpences",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_vehicleMaintenanceShedules_VehicleID",
                table: "vehicleMaintenanceShedules",
                column: "VehicleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingPaymentAllocations");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "overtimeLogs");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "salaries");

            migrationBuilder.DropTable(
                name: "vehicleExpences");

            migrationBuilder.DropTable(
                name: "vehicleMaintenanceShedules");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "ExternalEmployees");

            migrationBuilder.DropTable(
                name: "TravelAgents");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vehicles");
        }
    }
}

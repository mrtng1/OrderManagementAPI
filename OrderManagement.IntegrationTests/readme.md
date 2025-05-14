# Integration Tests: Order Creation

This document covers the SpecFlow integration tests for the `Create Order` feature in the Order Management system.

## Purpose

These tests verify that the `OrderService` correctly handles order creation, including:
- User and product validation.
- Stock level checks and updates.
- Delivery date calculations (considering business hours and weekends).
- Error handling for invalid scenarios.

## Test Structure

- **Feature File:** `PlaceOrder.feature` (describes test scenarios in Gherkin).
- **Step Definitions:** `PlaceOrderSteps.cs` (C# code implementing the Gherkin steps).
- **Dependencies:** Mocked repositories (`User`, `Product`, `Order`) to isolate `OrderService` logic.

## Key Scenarios Tested

- Successful order creation with stock updates.
- Invalid user or product IDs.
- Ordering products with zero quantity or insufficient stock.
- Delivery date logic for orders placed:
    - After closing time (16:00) on weekdays.
    - After closing time on weekends.
    - Before closing time on weekdays.
- Correct skipping of weekends for delivery dates.

## Running Tests

Use a .NET test runner (e.g., Visual Studio Test Explorer, `dotnet test`). Ensure NuGet packages are restored.

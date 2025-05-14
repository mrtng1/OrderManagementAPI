# API Integration Testing with Postman

This document outlines the Postman collection used for testing the Order Management API.

## Running Tests

These API tests are designed to be run using Newman, Postman's command-line Collection Runner.

1.  **Ensure Newman is installed:**
    If you don't have Newman, install it globally via npm:
    ```bash
    npm install -g newman
    ```

2.  **Run the collection:**
    Navigate to the directory containing your Postman collection file (e.g., `OrdersApiTests.json`) and run the following command:
    ```bash
    newman run OrdersApiTests.json
    ```
    Replace `OrdersApiTests.json` with the actual name of your collection file if it's different.

## API Endpoints Tested

This Postman collection includes tests for the following API endpoints:

* **`POST /api/orders?userId={userId}` - Create Order:**
    * Creates a new order for a specified user.
    * Tests for a successful response (status 200) and the presence of an `orderId` in the response.
    * The `orderId` is stored as an environment variable for use in subsequent requests.

* **`PUT /api/orders/{{orderId}}/scan-order` - Scan Order:**
    * Updates the status of an existing order (simulating a scan).
    * Depends on `orderId` from the "Create Order" request (uses a pre-request script to skip if `orderId` is not set).
    * Tests for a successful response (status 200) and the presence of a `newStatus` property.

* **`GET /api/orders/by-user/{userId}` - Get Orders by User:**
    * Retrieves all orders for a specific user.
    * Tests for a successful response (status 200) and that the response is an array.

* **`GET /api/orders/status/{{orderId}}` - Get Order Status:**
    * Retrieves the current status of a specific order.
    * Depends on `orderId` from the "Create Order" request (uses a pre-request script to skip if `orderId` is not set).
    * Tests for a successful response (status 200) and the presence of a `status` property in the response.

## Test Scripts

Each request in the collection contains JavaScript test scripts (under the "Tests" tab in Postman) to:
- Verify HTTP status codes.
- Check for specific properties in the JSON response.
- Set environment variables (e.g., `orderId`) to chain requests.
- Include pre-request scripts to manage test flow (e.g., skipping a request if a prerequisite like `orderId` is missing).

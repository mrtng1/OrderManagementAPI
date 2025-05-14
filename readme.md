# OrderManagementAPI 🛒

## 🌟 Project Overview

Welcome to the **OrderManagementAPI**! This project is a key component of the Software Quality course within the Software Development education program. The primary goal of this API is to serve as the backend for a webshop, allowing for the management of users, products, and orders, with a strong emphasis on software quality principles and practices.

This project showcases the implementation of a robust and testable API by adhering to **Clean Architecture** principles and employing various testing methodologies and tools.

## 🏗️ Architecture

The **OrderManagementAPI** is built following the principles of **Clean Architecture**. This architectural pattern helps in creating a system that is:

* **Independent of Frameworks:** The core business logic is not dependent on any specific framework.
* **Testable:** Business rules can be tested without the UI, Database, Web Server, or any other external element.
* **Independent of UI:** The UI can change easily, without changing the rest of the system.
* **Independent of Database:** You can swap out your data storage solution without affecting business rules.
* **Independent of any External Agency:** Business rules are self-contained and unaware of external specifics.

This separation of concerns leads to a more maintainable, scalable, and testable application.


## ✨ Core Features

The API provides the following functionalities:

* 👤 **User Management:**
    * Create new users.
    * Retrieve a list of all users.
* 🛍️ **Product Management:**
    * Create new products with details like name, price, and stock.
    * Retrieve a list of all available products.
* 📦 **Order Management:**
    * Allow users to place orders for one or more products.
    * Retrieve a list of all orders.
    * Retrieve orders specific to a user.
    * Track and update the status of an order (Created -> Delivery -> Delivered).
    * View the current status and estimated delivery time of an order.

## 📜 Business Rules

The API incorporates specific business logic, especially concerning order processing and delivery times:

* **Initial Delivery Estimate:** When a new order is created, the estimated delivery time is set to **2 business days** from the processing date.
* **Closing Time:** Orders are processed daily. The closing time for order processing is **16:00 (4 PM)**.
* **After Hours & Weekend Orders:**
    * If an order is placed *after* 16:00 on a weekday, it will be processed on the *next business day*.
    * If an order is placed after 16:00 on a Friday, or anytime on a Saturday or Sunday, it will be processed on the *following Monday*.
* **Stock Management:** Orders can only be fulfilled if there is sufficient stock for each product. Product stock is updated after an order is successfully placed.
* **Order Status Advancement:** The status of an order can be advanced (e.g., scanned for delivery). When an order status advances to `Delivery`, the estimated delivery time is updated to be 1 business day from the scan time (considering similar after-hours and weekend rules).

## 🛠️ Technologies & Quality Focus Areas

This project emphasizes software quality through the use of specific technologies, methodologies, and design considerations:

* 🧼 **Clean Architecture:** Forms the foundational structure, promoting separation of concerns and testability.
* 🔬 **Moq:** A mocking framework for .NET, used to isolate dependencies in unit tests.
* 🥒 **Specflow & Cucumber:** Behavior-Driven Development (BDD) is implemented, allowing acceptance tests in a human-readable format (Gherkin).
* 🛣️ **Path Testing:** A white-box testing technique to design test cases executing specific code paths.
* 👾 **Stryker .NET:** A mutation testing framework to assess the effectiveness of unit tests.
* 🧪 **Unit Testing:** Comprehensive unit tests to verify the correctness of individual code units.
* 📮 **Postman Testing:** Used for API endpoint testing, verifying functionality, response codes, and data integrity.
* 🧩 **Design for Testability:** The application is designed with practices like dependency injection and clear separation of concerns to enhance testability.

## ⚙️ API Endpoints

Here is a summary of the main API endpoints:

### Users Controller (`api/Users`)

* `GET /api/Users` - Retrieves all users.
* `POST /api/Users` - Creates a new user (Query param: `username`).

### Products Controller (`api/Products`)

* `GET /api/Products` - Retrieves all products.
* `POST /api/Products` - Creates a new product (Query params: `productName`, `price`, `stock`).

### Orders Controller (`api/Orders`)

* `GET /api/Orders` - Retrieves all orders.
* `POST /api/Orders` - Creates a new order (Query param: `userId`; Body: list of order items).
* `GET /api/Orders/by-user/{userId}` - Retrieves orders for a specific user.
* `GET /api/Orders/status/{id}` - Retrieves the status and delivery time of an order.
* `PUT /api/Orders/{id}/scan-order` - Advances the status of an order.

## 🚀 Getting Started (Example)

To get this project up and running (general steps, actual steps may vary):

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/mrtng1/OrderManagementAPI.git
    cd OrderManagementAPI
    ```
2.  **Restore dependencies:**
    ```bash
    dotnet restore
    ```
3.  **Build the project:**
    ```bash
    dotnet build
    ```
4.  **Run the API (from the WebApi project directory):**
    ```bash
    dotnet run
    ```
    (Assuming the entry point project is `OrderManagement.WebApi` or similar. If you are in the root, you might use `dotnet run --project PathToYourWebApiProjectFile`)
5.  The API should now be running, typically on `http://localhost:port` (e.g., `http://localhost:5000` or `https://localhost:5001`).

## 🧪 Testing Approach

Quality is ensured through a multi-faceted testing strategy:

* **Unit Tests:** Core services and business logic are unit tested using a framework like xUnit or MSTest, with Moq for dependency isolation.
* **Behavior-Driven Development (BDD):** Specflow and Gherkin describe system behavior from a user's perspective for acceptance testing.
* **API/Integration Tests:** Postman collections perform end-to-end tests on API endpoints.
* **Path Testing:** Critical code paths (like delivery time calculation) are specifically tested.
* **Mutation Testing:** Stryker .NET assesses unit test quality by introducing code mutations and checking if tests detect them.

This comprehensive approach aims for a reliable and high-quality OrderManagementAPI.

---

Enjoy managing your orders! 🎉
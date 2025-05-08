Feature: Create Order
  As a registered user, I want to create an order with products so that I can receive them at the correct time.

  Scenario: TC1 - Valid order with available stock
    Given a user exists with ID "f7cc96ba-e07a-4717-bcd0-3eff479b55ea"
    And the following products exist with stock:
      | ProductId                           | Name    | Stock |
      | f2aa11bb-3ac2-4d13-887e-123456789abc | Laptop  | 5     |
      | c8ddaaee-8dcb-4f6e-aaa1-abcdefabcdef | Monitor | 10    |
    When the user creates an order with the following items:
      | ProductId                           | Quantity |
      | f2aa11bb-3ac2-4d13-887e-123456789abc | 2        |
      | c8ddaaee-8dcb-4f6e-aaa1-abcdefabcdef | 1        |
    Then the order should be created successfully
    And the stock should be updated accordingly

  Scenario: TC2 - Invalid user
    When an order is created with user ID "f7cc96ba-e07a-4717-bcd0-3eff479b55ea"
    Then an error "User f7cc96ba-e07a-4717-bcd0-3eff479b55ea not found." should be thrown

  Scenario: TC3 - Product does not exist
    Given a user exists with ID "f7cc96ba-e07a-4717-bcd0-3eff479b55ea"
    When the user creates an order with the following items:
      | ProductId                           | Quantity |
      | ac3da5c8-f020-4cbc-9641-6028f2b21805 | 2        |
    Then an error "Product with ID ac3da5c8-f020-4cbc-9641-6028f2b21805 not found." should be thrown

  Scenario: TC4 - Product quantity is zero
    Given a user exists with ID "f7cc96ba-e07a-4717-bcd0-3eff479b55ea"
    And the following products exist with stock:
      | ProductId                            | Name        | Stock |
      | f2aa11bb-3ac2-4d13-887e-123456789abc | TestProduct | 5     |
    When the user creates an order with the following items:
      | ProductId                           | Quantity |
      | f2aa11bb-3ac2-4d13-887e-123456789abc | 0        |
    Then an error "Quantity for product 'TestProduct' must be positive." should be thrown

  Scenario: TC5 - Not enough stock
    Given a user exists with ID "f7cc96ba-e07a-4717-bcd0-3eff479b55ea"
    And the following products exist with stock:
      | ProductId                            | Name        | Stock |
      | f2aa11bb-3ac2-4d13-887e-123456789abc | TestProduct | 1     |
    When the user creates an order with the following items:
      | ProductId                           | Quantity |
      | f2aa11bb-3ac2-4d13-887e-123456789abc | 12        |
    Then an error "Not enough stock for product 'TestProduct' (requested: 12, available: 1)." should be thrown

#  Scenario: TC6 - Created after 16:00
#    Given the current time is "2025-05-08T17:00:00"
#    And a user exists with ID "f7cc96ba-e07a-4717-bcd0-3eff479b55ea"
#    And the following products exist with stock:
#      | ProductId                            | Name        | Stock |
#      | f2aa11bb-3ac2-4d13-887e-123456789abc | TestProduct | 10     |
#    When the user creates an order with the following items:
#      | ProductId                           | Quantity |
#      | f2aa11bb-3ac2-4d13-887e-123456789abc | 1        |
#    Then the delivery date should be 3 working days after "2025-05-09"
#
#  Scenario: TC7 - Created before 16:00
#    Given the current time is "2025-05-08T14:00:00"
#    And a user exists with ID "user-123"
#    And the product "prod-1" exists with stock 10
#    When the user creates an order with quantity 1 for product "prod-1"
#    Then the delivery date should be 3 working days after "2025-05-08"
#
#  Scenario: TC8 - Weekend and holidays are skipped
#    Given the current time is "2025-05-09T15:00:00" (Friday)
#    And a user exists with ID "user-123"
#    And the product "prod-1" exists with stock 10
#    When the user creates an order
#    Then the delivery date should skip Saturday and Sunday
#
#  Scenario: TC9 - Create order with multiple items and partial stock
#    Given a user exists with ID "user-123"
#    And the following products exist:
#      | ProductId | Stock |
#      | prod-1    | 2     |
#      | prod-2    | 0     |
#    When the user creates an order with the following items:
#      | ProductId | Quantity |
#      | prod-1    | 2        |
#      | prod-2    | 1        |
#    Then an error "Not enough stock for product 'prod-2'" should be thrown

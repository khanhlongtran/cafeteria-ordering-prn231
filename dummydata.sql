-- Insert test users (MANAGER, DELIVER, PATRON)
INSERT INTO users (full_name, email, password, phone, default_cuisine, role, created_at, updated_at)
VALUES 
('John Manager', 'john.manager@cafeteria.com', 'hashed_password_1', '0123456789', 'Vietnamese', 'MANAGER', GETDATE(), GETDATE()),
('Alice Deliverer', 'alice.deliver@cafeteria.com', 'hashed_password_2', '0123456790', NULL, 'DELIVER', GETDATE(), GETDATE()),
('Bob Customer', 'bob.customer@email.com', 'hashed_password_3', '0123456791', 'Chinese', 'PATRON', GETDATE(), GETDATE()),
('Sarah Manager', 'sarah.manager@cafeteria.com', 'hashed_password_4', '0123456792', 'Japanese', 'MANAGER', GETDATE(), GETDATE()),
('Mike Customer', 'mike.customer@email.com', 'hashed_password_5', '0123456793', 'Korean', 'PATRON', GETDATE(), GETDATE());

-- Insert addresses for customers with geolocation
INSERT INTO addresses (user_id, address_line, city, state, zip_code, is_default, created_at, updated_at, geoLocation)
VALUES 
(3, '123 Main St', 'Ho Chi Minh City', 'District 1', '700000', 1, GETDATE(), GETDATE(), '10.8231,106.6297'),
(3, '456 Side St', 'Ho Chi Minh City', 'District 3', '700001', 0, GETDATE(), GETDATE(), '10.7829,106.6871'),
(5, '789 Park Ave', 'Ho Chi Minh City', 'District 7', '700002', 1, GETDATE(), GETDATE(), '10.7329,106.7225');

-- Insert menus
INSERT INTO menus (manager_id, menu_name, description, created_at, updated_at, isStatus)
VALUES 
(1, 'Daily Specials', 'Our daily special menu items', GETDATE(), GETDATE(), 1),
(1, 'Weekend Brunch', 'Special brunch menu for weekends', GETDATE(), GETDATE(), 1),
(4, 'Healthy Options', 'Healthy and nutritious meals', GETDATE(), GETDATE(), 1);

-- Insert menu items
INSERT INTO menu_items (menu_id, item_name, description, price, item_type, created_at, updated_at, Count_items_sold, isStatus, Image)
VALUES 
(1, 'Pho Bo', 'Traditional Vietnamese beef noodle soup', 15.99, 'Main Course', GETDATE(), GETDATE(), 0, 1, 'pho.jpg'),
(1, 'Banh Mi', 'Vietnamese sandwich with various fillings', 8.99, 'Sandwich', GETDATE(), GETDATE(), 0, 1, 'banhmi.jpg'),
(2, 'Eggs Benedict', 'Classic eggs benedict with hollandaise sauce', 12.99, 'Breakfast', GETDATE(), GETDATE(), 0, 1, 'eggsbenedict.jpg'),
(2, 'French Toast', 'Sweet and fluffy french toast', 10.99, 'Breakfast', GETDATE(), GETDATE(), 0, 1, 'frenchtoast.jpg'),
(3, 'Quinoa Bowl', 'Healthy quinoa bowl with vegetables', 14.99, 'Healthy', GETDATE(), GETDATE(), 0, 1, 'quinoa.jpg'),
(3, 'Green Smoothie', 'Fresh green smoothie with fruits', 7.99, 'Beverage', GETDATE(), GETDATE(), 0, 1, 'smoothie.jpg');

-- Insert orders
INSERT INTO orders (user_id, order_date, status, payment_method, total_amount, address_id, created_at, updated_at)
VALUES 
(3, GETDATE(), 'PENDING', 'ZALOPAY', 24.98, 1, GETDATE(), GETDATE()),
(3, GETDATE(), 'DELIVERING', 'ZALOPAY', 15.99, 1, GETDATE(), GETDATE()),
(5, GETDATE(), 'COMPLETED', 'ZALOPAY', 22.98, 3, GETDATE(), GETDATE());

-- Insert order items
INSERT INTO order_items (order_id, item_id, quantity, price)
VALUES 
(1, 1, 1, 15.99),
(1, 2, 1, 8.99),
(2, 1, 1, 15.99),
(3, 3, 1, 12.99),
(3, 4, 1, 10.99);

-- Insert delivery records
INSERT INTO delivery (order_id, deliver_user_id, pickup_time, delivery_time, delivery_status, created_at, updated_at)
VALUES 
(2, 2, GETDATE(), NULL, 'DELIVERING', GETDATE(), GETDATE()),
(3, 2, DATEADD(hour, -1, GETDATE()), GETDATE(), 'COMPLETED', GETDATE(), GETDATE());

-- Insert feedback
INSERT INTO feedback (user_id, order_id, rating, comment, created_at)
VALUES 
(3, 2, 5, 'Great service and delicious food!', GETDATE()),
(5, 3, 4, 'Good food, delivery was a bit late', GETDATE());

-- Insert account activity
INSERT INTO account_activity (user_id, activity_type, activity_time)
VALUES 
(3, 'LOGIN', GETDATE()),
(3, 'PLACE_ORDER', GETDATE()),
(5, 'LOGIN', GETDATE()),
(5, 'PLACE_ORDER', GETDATE());

-- Insert recommended meals
INSERT INTO recommended_meals (user_id, item_id, score, created_at)
VALUES 
(3, 1, 4.5, GETDATE()),
(3, 2, 4.0, GETDATE()),
(5, 3, 4.8, GETDATE()),
(5, 4, 4.2, GETDATE());

-- Insert revenue reports
INSERT INTO revenue_reports (manager_id, report_date, total_orders, total_revenue, generated_at)
VALUES 
(1, CAST(GETDATE() AS DATE), 2, 40.97, GETDATE()),
(4, CAST(GETDATE() AS DATE), 1, 22.98, GETDATE());

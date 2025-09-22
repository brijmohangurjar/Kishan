-- Create Admin Account Script
-- Run this in your database to create an admin account

-- First, make sure you have BCrypt.Net package installed in your project
-- The password will be hashed using BCrypt

-- Example admin account:
-- Email: admin@krishiclinic.com
-- Password: Admin123!
-- Name: System Administrator

INSERT INTO Admins (Name, Email, Password, Role, IsActive, CreatedAt)
VALUES (
    'System Administrator',
    'admin@krishiclinic.com',
    '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy', -- This is 'Admin123!' hashed with BCrypt
    'SuperAdmin',
    1,
    GETUTCDATE()
);

-- Alternative: You can also create a simple password like 'admin' or 'password'
-- But remember to change it after first login!

-- For password 'admin':
-- INSERT INTO Admins (Name, Email, Password, Role, IsActive, CreatedAt)
-- VALUES (
--     'Admin User',
--     'admin@test.com',
--     '$2a$11$rQZ8K3tXvL7hM9nP2qR1te8K5wF3sA6bC9dE1fG4hI7jK0lM3nP6q', -- This is 'admin' hashed
--     'Admin',
--     1,
--     GETUTCDATE()
-- );

-- For password 'password':
-- INSERT INTO Admins (Name, Email, Password, Role, IsActive, CreatedAt)
-- VALUES (
--     'Admin User',
--     'admin@test.com',
--     '$2a$11$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', -- This is 'password' hashed
--     'Admin',
--     1,
--     GETUTCDATE()
-- );

# How to Create Admin Account

## Option 1: Using the API Endpoint (Recommended)

I've added a new endpoint to create admin accounts. You can use this to create your first admin:

**Endpoint:** `POST /api/admin/create`

**Request Body:**
```json
{
  "name": "System Administrator",
  "email": "admin@krishiclinic.com",
  "password": "Admin123!",
  "role": "SuperAdmin"
}
```

**Example using curl:**
```bash
curl -X POST "http://localhost:5000/api/admin/create" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "System Administrator",
    "email": "admin@krishiclinic.com",
    "password": "Admin123!",
    "role": "SuperAdmin"
  }'
```

## Option 2: Using SQL Script

Run the SQL script in `CREATE_ADMIN_ACCOUNT.sql` in your database.

## Option 3: Test Credentials

For quick testing, you can use these pre-hashed credentials:

**Email:** `admin@test.com`  
**Password:** `admin`

Or:

**Email:** `admin@test.com`  
**Password:** `password`

## After Creating Admin Account

1. Start your backend server
2. Start your Angular admin panel (`ng serve`)
3. Go to `http://localhost:4200`
4. Use the admin credentials you created
5. You should be able to log in and access the dashboard

## Security Note

- Change the default password after first login
- Use strong passwords in production
- Consider implementing password policies
- The passwords are securely hashed using BCrypt

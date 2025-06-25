# 🌐 Environment Configuration

To run the backend application correctly, you need to configure the following environment variables.

Create a `.env` file in the root of your project and add the following values:

```env
# MongoDB connection string
MYAPP_MONGODB_CONNECTION=mongodb+srv://<USERNAME>:<PASSWORD>@cluster0.tng6fsx.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0

# JWT secret key used for signing authentication tokens
MYAPP_JWT_SECRET_KEY=your-very-secret-jwt-key

# Port number the server should run on
MYAPP_PORT=10000

# Comma-separated list of allowed frontend URLs for CORS
MYAPP_FRONTEND_URLS=http://localhost:4200,https://book-app-delta-hazel.vercel.app

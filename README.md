# UrlShortenService

A URL shortening service built with ASP.NET Core Web API.
https://roadmap.sh/projects/url-shortening-service

## Features

- Shorten long URLs
- Redirect to original URLs
- Validate URL format
- Generate unique short codes
- Track access count
- Retrieve URL statistics

## Technologies

- ASP.NET Core Web API
- Entity Framework Core
- MySQL

## Endpoints

### Create Short URL

Creates a new shortened URL.

```http
POST /api/shorten
```

Request Body

```json
{
  "url": "https://www.github.com"
}
```

### Get URL Information

Retrieves information about a shortened URL.

```http
GET /api/shorten/{shortCode}
```

### Get URL Statistics

Returns statistics for a shortened URL, including access count.

```http
GET /api/shorten/{shortCode}/stats
```

### Redirect to Original URL

Redirects the client to the original URL associated with the short code.

```http
GET /api/{shortCode}
```

### Update URL

Updates the original URL associated with a short code.

```http
PUT /api/shorten/{shortCode}
```

Request Body

```json
{
  "url": "https://www.github.com/updated"
}
```

### Delete URL

Deletes an existing shortened URL.

```http
DELETE /api/shorten/{shortCode}
```

### Create Short URL

POST /api/urls

### Get URL Statistics

GET /api/urls/stats/{shortCode}

### Redirect

GET /{shortCode}



## Running the Project

1. Configure the database connection.
2. Update the connection string if required.
3. Run the application:

```bash
dotnet run
```


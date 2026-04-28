# Libreroo Web

## Run

From this folder:

```bash
npm install
npm start
```

Open `http://localhost:4200`.

## Backend requirement

The API must be running on `http://localhost:5141` (default proxy target).  
The Angular dev server is configured to proxy `/books`, `/authors`, `/members`, and `/loans` to the API.

## Tests

```bash
npm test
```

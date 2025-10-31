export default function NotFoundPage() {
    return (
        <div className="d-flex flex-column justify-content-center align-items-center min-vh-100 text-center">
            <h1 className="display-1 fw-bold text-danger">404</h1>
            <h2 className="mb-3">Page not found</h2>
            <p className="text-muted mb-4">
                The page you’re looking for doesn’t exist or has been moved.
            </p>
            <a href="/" className="btn btn-primary">
                <i className="bi bi-house-door me-2"></i> Back to Home
            </a>
        </div>
    );
}

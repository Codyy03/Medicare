export default function ServerErrorPage() {
    return (
        <div className="d-flex flex-column justify-content-center align-items-center min-vh-100 text-center">
            <h1 className="display-1 fw-bold text-secondary">500</h1>
            <h2 className="mb-3">Something went wrong</h2>
            <p className="text-muted mb-4">
                An unexpected error occurred on our side. Please try again later.
            </p>
            <a href="/" className="btn btn-outline-secondary">
                <i className="bi bi-arrow-repeat me-2"></i> Refresh or Go Home
            </a>
        </div>
    );
}

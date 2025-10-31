export default function UnauthorizedPage() {
    return (
        <div className="d-flex flex-column justify-content-center align-items-center min-vh-100 text-center">
            <h1 className="display-1 fw-bold text-warning">401</h1>
            <h2 className="mb-3">Unauthorized</h2>
            <p className="text-muted mb-4">
                You don’t have permission to view this page. Please log in to continue.
            </p>
            <a href="/login" className="btn btn-outline-primary">
                <i className="bi bi-box-arrow-in-right me-2"></i> Go to Login
            </a>
        </div>
    );
}

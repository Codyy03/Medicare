import { useLocation, Link } from "react-router-dom";

const BookingSuccessPage = () => {
    const location = useLocation();
    const { specialization, doctor, timeSlot, room } = location.state || {};

    return (
        <main className="container my-5">
            <div className="card shadow-sm p-5 text-center">
                <h2 className="text-primary mb-4">The visit has been successfully registered</h2>
                <p className="lead">
                    You can check the details in your profile or in your confirmation email.
                </p>

                <div className="mt-4 text-start">
                    <h5>Details of the visit:</h5>
                    <ul className="list-unstyled">
                        <li><strong>Specialization:</strong> {specialization}</li>
                        <li><strong>Doctor:</strong> {doctor}</li>
                        <li><strong>Hour:</strong> {timeSlot}</li>
                        <li><strong>Room:</strong> {room}</li>
                    </ul>
                </div>

                <div className="mt-4">
                    <Link to="/patient/visits" className="btn btn-primary me-2">Go to profile</Link>
                    <Link to="/" className="btn btn-outline-secondary">Home page</Link>
                </div>
            </div>
        </main>
    );
};

export default BookingSuccessPage;

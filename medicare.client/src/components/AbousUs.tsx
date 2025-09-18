import { Link } from "react-router-dom";
import "./AboutUs.css";

function AboutUs() {
    return (
        <div className="about-us container my-5 ">
            <div className="row justify-content-center align-items-center">
                <div className="col-md-6 text-center text-md-start">
                    <h2 className="mb-4">
                        Over 20 years <br /> together with our patients
                    </h2>
                    <p className="mb-4">
                        For two decades, we have been combining experience, modern
                        technology, and a personalized approach to provide our patients with
                        the highest level of care. Our goal is not only treatment, but also
                        prevention and supporting a healthy lifestyle.
                    </p>
                    <Link to="/about" className="btn-anim">
                        <span>More &gt;</span>
                    </Link>
                </div>

                <div className="col-md-4 mt-4 mt-md-0">
                    <img
                        src="/doctor.png"
                        className="img-fluid rounded shadow-sm"
                        alt="doctor"
                    />
                </div>
            </div>
        </div>
    );
}

export default AboutUs;

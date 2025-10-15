import "./AboutUs.css";

export default function AboutUs() {
    return (
        <div className="about-container">
            <section className="about-hero">
                <div className="about-hero-content">
                    <h1>About us</h1>
                    <p>Your health is most important to us.</p>
                </div>
            </section>

            <section className="about-content container">
                <h2>Over 20 years of caring for the health of our patients</h2>
                <p>
                    For two decades, our clinic has been providing comprehensive medical care to residents of the entire region. Thanks to a team of experienced physicians and modern diagnostic facilities, we can provide services at the highest level, both under contracts with the National Health Fund and privately.
                </p>

                <img src="/doctors-aboutUs.png" className="aboutUs-images" alt="Our doctors" />

                <p>
                    We serve hundreds of patients every day, offering a wide range of examinations, specialist consultations, and preventive programs. Our mission is not only treatment, but also health education and promoting a healthy lifestyle.
                </p>

                <section className="about-equipment">
                    <div className="container">
                        <h2>We have modern medical equipment:</h2>
                        <div className="equipment-grid">
                            <ul>
                                <li>Ultrasound machines</li>
                                <li>Echocardiographs</li>
                                <li>Endoscopes</li>
                                <li>Mammographs</li>
                                <li>Laboratory equipment</li>
                            </ul>
                            <ul>
                                <li>Ophthalmic equipment</li>
                                <li>Dental equipment</li>
                                <li>Surgical instruments</li>
                                <li>Cardiology equipment</li>
                                <li>Rehabilitation equipment</li>
                            </ul>
                        </div>
                    </div>
                </section>

                <img src="/clinic-aboutUs.jpg" className="aboutUs-images" alt="Clinic" />

                <p>
                    Our team consists of over 100 specialists in various fields doctors, nurses, diagnosticians, and physiotherapists who put their hearts into their work every day.
                </p>

                <div className="about-mission">
                    <h3>Mission</h3>
                    <p>
                        Provide patients with the highest level of medical care, while respecting their time, needs, and comfort.
                    </p>
                    <h3>Vision</h3>
                    <p>
                        To be a regional leader in modern, accessible, and patient-friendly healthcare.
                    </p>
                </div>
            </section>
        </div>
    );
}

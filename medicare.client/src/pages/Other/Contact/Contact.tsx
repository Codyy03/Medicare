import "./Contact.css";

export default function Contact() {
    return (
        <div className="contact-container">
            {/* HERO */}
            <section className="contact-hero">
                <div className="contact-content">
                    <h1>Contact</h1>
                </div>
            </section>

            {/* Main contact page */}
            <section className="contact-main container">
                {/* Left column */}
                <div className="contact-info">
                    <div className="contact-block">
                        <h2 className="contact-title">Contact Information</h2>
                        <p><strong>Address:</strong> Superb Street 1, 35-055 Rzeszow</p>
                        <p><strong>Phone:</strong> 6532 542 5123</p>
                        <p><strong>Email:</strong> medicare@email.com</p>
                        <p><strong>Opening hours:</strong> Mon&ndash;Fri 8:00&ndash;18:00</p>
                    </div>
                </div>

                {/* Form */}
                <div className="contact-form">
                    <h2>SEND US A MESSAGE</h2>
                    <form>
                        <input type="text" placeholder="Name" required />
                        <input type="email" placeholder="Email" required />
                        <input type="text" placeholder="Subject" />
                        <textarea placeholder="Message" rows={5} required></textarea>
                        <button type="submit">Send</button>
                    </form>
                </div>
            </section>
            {/* Map */}
            <section className="contact-map-section">
                <h2 className="map-title">Clinic location</h2>
                <div className="map-wrapper">
                    <iframe
                        title="Our location"
                        src="https://www.google.com/maps/embed?pb=!1m17!1m12!1m3!1d3209.8948673075756!2d22.001205077124926!3d50.038017971516425!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m2!1m1!2zNTDCsDAyJzE2LjkiTiAyMsKwMDAnMTMuNiJF!5e1!3m2!1spl!2spl!4v1758479836868!5m2!1spl!2spl"
                        width="100%"
                        height="400"
                        style={{ border: 0 }}
                        allowFullScreen
                        loading="lazy"
                    ></iframe>
                </div>
            </section>
        </div>
    );
}

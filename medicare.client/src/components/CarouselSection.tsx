import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';
import './CarouseSection.css';

function CarouselSection() {
  return (
      <div id="mainCarousel" className="carousel slide" data-bs-ride="carousel" data-bs-interval="5000">

          <div className="carousel-indicators">
              <button type="button" data-bs-target="#mainCarousel" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
              <button type="button" data-bs-target="#mainCarousel" data-bs-slide-to="1" aria-label="Slide 2"></button>
              <button type="button" data-bs-target="#mainCarousel" data-bs-slide-to="2" aria-label="Slide 3"></button>
          </div>

          <div className="carousel-inner">
              <div className="carousel-item active">
                  <img src="/slide1.png" className="d-block w-100" alt="..." />
              </div>
              <div className="carousel-item">
                  <img src="/slide2.png" className="d-block w-100" alt="..." />
              </div>
              <div className="carousel-item">
                  <img src="/slide3.png" className="d-block w-100" alt="..." />
              </div>
          </div>

          <button className="carousel-control-prev" type="button" data-bs-target="#mainCarousel" data-bs-slide="prev">
              <span className="carousel-control-prev-icon" aria-hidden="true"></span>
              <span className="visually-hidden">Previous</span>
          </button>
          <button className="carousel-control-next" type="button" data-bs-target="#mainCarousel" data-bs-slide="next">
              <span className="carousel-control-next-icon" aria-hidden="true"></span>
              <span className="visually-hidden">Next</span>
          </button>
      </div>

  );
}

export default CarouselSection;
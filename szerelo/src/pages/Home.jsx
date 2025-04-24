import React from 'react';
import { Link } from 'react-router-dom';
import "../styles/home.css";
import workshopImage1 from '../images/pexels-cottonbro-4489794.jpg';
import workshopImage2 from '../images/pexels-cottonbro-4489743.jpg';
import workshopImage3 from '../images/pexels-cottonbro-4489707.jpg';
import { GiFlatTire } from "react-icons/gi";
import { FaWrench, FaOilCan, FaCarCrash, FaSlidersH, FaSearch, FaSnowflake } from "react-icons/fa";

const Home = () => {
  return (
    <div className="home">
      <section className="hero">
        <div className="hero-content">
          <h1 className='cim'>Üdvözöljük az Autószervizünkben!</h1>
          <p className='cim'>Megbízható és professzionális autójavítás Szombathelyen. Szakértő csapatunk várja, hogy segíthessen járműve karbantartásában és javításában!</p>
          <Link to="/Booking" className="cta-button">Időpontfoglalás</Link>
        </div>
      </section>

      <section className="about-us">
        <div className="container">
          <h2>Miért Minket Válasszon?</h2>
          <div className="features">
            <div className="feature-item">
              <i className="fas fa-tools"></i>
              <h3>Szakértelem</h3>
              <p>Tapasztalt szerelőink a legmodernebb diagnosztikai és javító eszközökkel dolgoznak, biztosítva a precíz és hatékony munkát.</p>
            </div>
            <div className="feature-item">
              <i className="fas fa-check-circle"></i>
              <h3>Megbízhatóság</h3>
              <p>Gyors, átlátható és precíz munkavégzés, minden elvégzett javításra garanciát vállalunk. Nincsenek rejtett költségek.</p>
            </div>
            <div className="feature-item">
              <i className="fas fa-handshake"></i>
              <h3>Ügyfélközpontúság</h3>
              <p>Az Ön elégedettsége a legfontosabb számunkra. Barátságos kiszolgálással és személyre szabott megoldásokkal várjuk.</p>
            </div>
          </div>
        </div>
      </section>

      <section className="services-overview">
        <div className="container">
          <h2>Kiemelt Szolgáltatásaink</h2>
          <p className="section-intro">Szolgáltatásaink széles skáláját kínáljuk járműve optimális állapotának megőrzéséhez.</p>
          <ul>
            <li><i className="service-icon"><FaWrench /></i>Általános karbantartás</li>
            <li><i className="service-icon"><FaOilCan /></i>Olajcsere</li>
            <li><i className="fas fa-car-side service-icon"></i>Futómű beállítás és javítás</li>
            <li><i className="fas fa-laptop-medical service-icon"></i>Diagnosztika és hibakód olvasás</li>
            <li><i className="service-icon"><GiFlatTire /></i>Gumicsere és centrírozás</li>
            <li><i className="fas fa-clipboard-list service-icon"></i>Műszaki vizsgára felkészítés</li>
          </ul>
          <Link to="/services" className="more-services-link">Összes Szolgáltatásunk Megtekintése</Link>
        </div>
      </section>

      <section className="testimonials">
        <div className="container">
          <h2>Vélemények</h2>
          <div className="testimonial-items">
            <div className="testimonial-item">
              <p>"Nagyon elégedett vagyok a szerviz munkájával. Gyorsak, profik és nagyon kedvesek voltak. Csak ajánlani tudom!"</p>
              <p className="customer-name">- Kovács Anna</p>
            </div>
            <div className="testimonial-item">
              <p>"Már többször jártam itt, mindig kifogástalan munkát végeztek. Megbízható autószerviz Szombathelyen!"</p>
              <p className="customer-name">- Nagy Péter</p>
            </div>
          </div>
        </div>
      </section>

      <section className="gallery">
        <div className="container">
          <h2>Műhelyünk és Munkánk</h2>
          <div className="gallery-grid">
            <img src={workshopImage1} alt="Autószerviz Műhely 1" className="gallery-item" />
            <img src={workshopImage2} alt="Autószerviz Műhely 2" className="gallery-item" />
            <img src={workshopImage3} alt="Autószerviz Műhely 3" className="gallery-item" />
          </div>
          <p className="gallery-caption">Tekintsen be műhelyünkbe, ahol modern eszközökkel és precizitással dolgozunk.</p>
        </div>
      </section>

      <section className="contact-prompt">
        <div className="container">
          <h2>Kérdése van vagy időpontot foglalna?</h2>
          <p>Lépjen velünk kapcsolatba még ma! Szakértő csapatunk készséggel áll rendelkezésére.</p>
          <p><i className="fas fa-phone-alt contact-icon"></i>Telefonszám: <strong>+36 30 123 4567</strong></p>
          <p><i className="fas fa-envelope contact-icon"></i>Email: <strong>balint.avar@gmail.com</strong></p>
          <p><i className="fas fa-map-marker-alt contact-icon"></i>Cím: <strong>Szombathely, Petőfi Utca 25.</strong></p>

          <div className="contact-buttons">
            <Link to="/Booking" className="cta-button">Időpontfoglalás Online</Link>
            <Link to="/contact" className="cta-button secondary">Írjon Nekünk Üzenetet</Link>
          </div>
        </div>
      </section>

    </div>
  );
};

export default Home;
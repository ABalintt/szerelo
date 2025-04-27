import React from 'react';
import '../styles/footer.css';
import { FaFacebookF, FaInstagram, FaXTwitter, FaTiktok } from 'react-icons/fa6';
import { Link } from 'react-router-dom';
import logo from '../images/logo-white.png';

const Footer = () => {
  const currentYear = new Date().getFullYear();

  return (
    <footer className="footer">
      <div className="footer_container">
        <div className="footer_about">
        <img src={logo} alt="Autószerelő Műhely Logó" className="footer_logo" />
        <h3 className="footer_title">Futóműhely</h3>
          <p>Minőségi szervizelés és gyors javítás egy helyen.</p>
        </div>

        <div className="footer_links">
        <Link to={"/services"}><h4 className="footer_subtitle">Szolgáltatások</h4></Link>
          <ul>
            <li>Olajcsere</li>
            <li>Fékjavítás</li>
            <li>Motorjavítás</li>
            <li>Gumi- és kerék csere</li>
          </ul>
        </div>

        <div className="footer_contact">
          <h4 className="footer_subtitle"><Link to={"/contact"}>Elérhetőségek</Link> </h4>
          <ul>
            <li>Cím: <a href="https://maps.app.goo.gl/H4b4DEUxH4yRfBqNA" target="_blank"> 9700 Szombathely, Petőfi utca 25.</a></li>
            <li>Telefon: <a href="tel:+3612345678">+36 30 123 4567</a></li>
            <li>Email: <a href="mailto:balint.avar@gmail.com">balint.avar@gmail.com</a></li>
          </ul>
        </div>

        <div className="footer_social">
          <h4 className="footer_subtitle">Kövess minket</h4>
          <div className="social-icons">
            <a href="https://facebook.com" target="_blank" rel="noopener noreferrer" aria-label="Facebook">
              <FaFacebookF />
            </a>
            <a href="https://instagram.com" target="_blank" rel="noopener noreferrer" aria-label="Instagram">
              <FaInstagram />
            </a>
            <a href="https://x.com" target="_blank" rel="noopener noreferrer" aria-label="X">
              <FaXTwitter />
            </a>
            <a href="https://tiktok.com" target="_blank" rel="noopener noreferrer" aria-label="Tiktok">
              <FaTiktok />
            </a>
          </div>
        </div>
      </div>

      <div className="footer_bottom">
        <p>© {currentYear} Futóműhely. Minden jog fenntartva.</p>
      </div>
    </footer>
  );
};

export default Footer;

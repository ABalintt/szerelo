import { useState } from 'react';
import { NavLink } from "react-router-dom";
import "../styles/navbar.css";
import { FaBars, FaTimes, FaHome, FaWrench, FaCalendarAlt, FaEnvelope } from "react-icons/fa";
import Logo from '../images/logo-white.png';

const Navbar = () => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const toggleMobileMenu = () => {
    setIsMobileMenuOpen(!isMobileMenuOpen);
  };

  const closeMobileMenu = () => {
    setIsMobileMenuOpen(false);
  }

  return (
    <nav className='navbar'>
      <NavLink to="/" className="navbar-logo" onClick={closeMobileMenu} end>
        <img src={Logo} alt="Autószerviz Logo" className="navbar-logo-img" />
      </NavLink>

      <div className="navbar-controls">
        <ul className={isMobileMenuOpen ? "nav-menu active" : "nav-menu"}>
          <li className="nav-item">
            <NavLink to="/" className="nav-link" onClick={closeMobileMenu} end>
              <FaHome /> Főoldal
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink to="/services" className="nav-link" onClick={closeMobileMenu}>
              <FaWrench /> Szolgáltatások
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink to="/booking" className="nav-link" onClick={closeMobileMenu}>
              <FaCalendarAlt /> Időpontfoglalás
            </NavLink>
          </li>
          <li className="nav-item">
            <NavLink to="/contact" className="nav-link" onClick={closeMobileMenu}>
              <FaEnvelope /> Kapcsolat
            </NavLink>
          </li>
        </ul>

        <div className="navbar-icons">
          <button className="mobile-menu-icon" onClick={toggleMobileMenu} aria-label={isMobileMenuOpen ? "Menü bezárása" : "Menü megnyitása"} aria-expanded={isMobileMenuOpen}>
            {isMobileMenuOpen ? <FaTimes /> : <FaBars />}
          </button>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;
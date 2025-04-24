import "../styles/contact.css";
import React, { useState } from 'react';

const Contact = () => {
  const [formData, setFormData] = useState({
    name: '',
    email: '',
    phone: '',
    message: '',
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prevState => ({
      ...prevState,
      [name]: value,
    }));
  };

  return (
    <div className="contact-container">
      <h1>Kapcsolat</h1>

      <div className="contact-content">
        <form className="contact-form" /*action="https://formspree.io/f/manelnkz"*/ method="POST">
          <h2>Írjon nekünk!</h2>
          <div className="form-group">
            <label htmlFor="name">Név:</label>
            <input
              type="text"
              id="name"
              name="name"
              value={formData.name}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="email">Email:</label>
            <input
              type="email"
              id="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="phone">Telefonszám:</label>
            <input
              type="tel"
              id="phone"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
            />
          </div>

          <div className="form-group">
            <label htmlFor="message">Üzenet:</label>
            <textarea
              id="message"
              name="message"
              rows="5"
              value={formData.message}
              onChange={handleChange}
              required
            ></textarea>
          </div>

          <button type="submit" className="submit-btn">
            Küldés
          </button>
        </form>

        <div className="contact-info">
          <h2>Elérhetőségeink</h2>
          <p>
            <i className="fas fa-map-marker-alt"></i> <a href="https://maps.app.goo.gl/H4b4DEUxH4yRfBqNA" target="_blank">9700 Szombathely, Petőfi utca 25.</a>
          </p>
          <p>
            <i className="fas fa-phone"></i> <a href="tel:+3612345678">+36 30 123 4567</a>
          </p>
          <p>
            <i className="fas fa-envelope"></i> <a href="mailto:info@autoszerelo.hu">balint.avar@gmail.com</a>
          </p>
          <p>
            <i className="fas fa-clock"></i>
            Hétfő-Péntek: 8:00 - 16:00 <br />
            Szombat és Vasárnap: Zárva
          </p>
        </div>
      </div>

      <div className="map-container">
        <iframe
          title="Autószerelő műhely térkép"
          src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d1354.6944046068695!2d16.62326066121028!3d47.228540115991464!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x476eb9d57f3c48eb%3A0xffb223d4850dca93!2sHorv%C3%A1th%20Boldizs%C3%A1r%20K%C3%B6zgazdas%C3%A1gi%20%C3%A9s%20Informatikai%20Technikum!5e0!3m2!1shu!2shu!4v1743943854788!5m2!1shu!2shu"
          allowFullScreen=""
          loading="lazy"
          referrerPolicy="no-referrer-when-downgrade"
        ></iframe>
      </div>
    </div>
  );
};

export default Contact;
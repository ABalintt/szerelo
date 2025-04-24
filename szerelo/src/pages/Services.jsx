import React from "react";
import "../styles/services.css";
import { Link } from "react-router-dom";
import { FaWrench, FaOilCan, FaCarCrash, FaSlidersH, FaSearch, FaSnowflake } from "react-icons/fa";

const Services = () => {
  const servicesList = [
    {
      id: 1,
      title: "Általános karbantartás",
      description: "Rendszeres karbantartás és ellenőrzés a járműve optimális állapotának megőrzéséhez.",
      price: "15.000 Ft-tól",
      icon: FaWrench
    },
    {
      id: 2,
      title: "Olajcsere",
      description: "Professzionális olajcsere szolgáltatás minőségi olajokkal és szűrőkkel.",
      price: "12.000 Ft-tól",
      icon: FaOilCan
    },
    {
      id: 3,
      title: "Fékrendszer javítás",
      description: "Fékbetétek, féktárcsák cseréje és fékrendszer teljes átvizsgálása.",
      price: "25.000 Ft-tól",
      icon: FaCarCrash
    },
    {
      id: 4,
      title: "Futómű beállítás",
      description: "Precíz futómű beállítás a jobb úttartás és az egyenletes gumikopás érdekében.",
      price: "20.000 Ft-tól",
      icon: FaSlidersH
    },
    {
      id: 5,
      title: "Diagnosztika",
      description: "Korszerű számítógépes diagnosztika a hibák pontos feltárásához.",
      price: "8.000 Ft-tól",
      icon: FaSearch
    },
    {
      id: 6,
      title: "Klíma szerviz",
      description: "Teljes klíma rendszer átvizsgálása, tisztítása és feltöltése.",
      price: "18.000 Ft-tól",
      icon: FaSnowflake
    }
  ];

  return (
    <div className="services-container">
      <div className="services-header">
        <h1>Szolgáltatásaink</h1>
        <p>Műhelyünk teljes körű autószerviz szolgáltatásokat kínál, személyre szabott megoldásokkal.</p>
      </div>

      <div className="services-grid">
        {servicesList.map((service) => (
          <div className="service-card" key={service.id}>
            <div className="service-icon">
              {typeof service.icon === 'function' ? <service.icon /> : null}
            </div>
            <h3>{service.title}</h3>
            <p>{service.description}</p>
            <div className="service-price">
              <span>Ár: {service.price}</span>
            </div>

            <Link to={"/Booking"} className="service-button">Időpontfoglalás</Link>
          </div>
        ))}
      </div>

      <div className="services-cta">
        <h2>Nem találja amit keres?</h2>
        <p>Vegye fel velünk a kapcsolatot egyedi igényeivel!</p>
        <Link to={"/Contact"} className="contact-button">Kapcsolatfelvétel</Link>
      </div>
    </div>
  );
};

export default Services;
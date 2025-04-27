import React, { useEffect } from "react";
import "../styles/services.css";
import { Link } from "react-router-dom";
import { FaWrench, FaOilCan, FaCarCrash, FaSlidersH, FaSearch, FaSnowflake, FaCar, FaBatteryFull, FaCarSide, FaTint, FaGlassWhiskey, FaBolt, FaClipboardCheck } from "react-icons/fa";

const Services = () => {

  useEffect(() => {
    window.scrollTo(0, 0);
  }, []);
  
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
    },
    {
      id: 7,
      title: "Gumicsere és szezonális tárolás",
      description: "Gumiabroncsok szakszerű cseréje és igény esetén szezonális tárolása.",
      price: "10.000 Ft-tól",
      icon: FaCar
    },
    {
      id: 8,
      title: "Akkumulátor csere és ellenőrzés",
      description: "Az akkumulátor állapotának vizsgálata és szükség esetén cseréje.",
      price: "15.000 Ft-tól",
      icon: FaBatteryFull
    },
    {
      id: 9,
      title: "Kipufogórendszer javítás",
      description: "Kipufogórendszer elemeinek javítása vagy cseréje.",
      price: "20.000 Ft-tól",
      icon: FaCarSide
    },
    {
      id: 10,
      title: "Fékfolyadék csere",
      description: "A fékfolyadék rendszeres cseréje a fékhatás optimális szinten tartásához.",
      price: "10.000 Ft-tól",
      icon: FaTint
    },
    {
      id: 11,
      title: "Hűtőrendszer javítás",
      description: "Hűtő, vízpumpa, termosztát és csövek ellenőrzése, javítása vagy cseréje.",
      price: "25.000 Ft-tól",
      icon: FaSnowflake
    },
    {
      id: 12,
      title: "Szélvédő javítás/csere",
      description: "Kisebb sérülések javítása vagy teljes szélvédő csere.",
      price: "15.000 Ft-tól",
      icon: FaGlassWhiskey
    },
    {
      id: 13,
      title: "Elektromos rendszer hibaelhárítás",
      description: "Autó elektromos rendszereinek vizsgálata és javítása.",
      price: "12.000 Ft-tól",
      icon: FaBolt
    },
    {
      id: 14,
      title: "Vásárlás előtti átvizsgálás",
      description: "Használt autó teljes körű átvizsgálása vásárlás előtt.",
      price: "20.000 Ft-tól",
      icon: FaClipboardCheck
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

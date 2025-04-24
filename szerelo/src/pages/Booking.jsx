import React, { useState, useEffect } from 'react';
import DatePicker, { registerLocale } from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import styles from '../styles/booking.module.css';
import { carData } from './carData';
import { hu } from 'date-fns/locale';
import { isBefore, startOfDay } from 'date-fns';
import emailjs from '@emailjs/browser';

registerLocale('hu', hu);

const allMakes = Object.keys(carData);

const Booking = () => {
  const [formData, setFormData] = useState({
    service: '', date: null, time: '', name: '', Email_cim: '', phone: '', address: '', licensePlate: '', vin: '', make: '', model: '', year: '', mileage: ''
  });

  const [makeSuggestions, setMakeSuggestions] = useState([]);
  const [showMakeSuggestions, setShowMakeSuggestions] = useState(false);
  const [modelSuggestions, setModelSuggestions] = useState([]);
  const [showModelSuggestions, setShowModelSuggestions] = useState(false);

  const [allAppointments, setAllAppointments] = useState([]);
  const [availableTimes, setAvailableTimes] = useState([]);
  const [errors, setErrors] = useState({});
  const [isSubmitted, setIsSubmitted] = useState(false);

  const servicesDurations = { 'Általános karbantartás, Olajcsere': 60, 'Fékrendszer javítás': 90, 'Futómű-beállítás': 120, 'Diagnosztika': 45, 'Klíma szerviz': 75, 'Egyéb': 60 };

  const services = Object.keys(servicesDurations);

  const workingHours = {
    start: 8, end: 16, interval: 30
  };

  const fetchAppointments = async () => {
    try {
      const response = await fetch('https://localhost:44364/api/Idopont');
      if (!response.ok) throw new Error('Hiba az adatlekéréskor');
      return await response.json();
    } catch (error) {
      console.error('API hiba:', error);
      return [];
    }
  };

  useEffect(() => {
    const loadAppointments = async () => {
      const data = await fetchAppointments();
      setAllAppointments(data);
    };
    loadAppointments();
  }, []);

  const isWeekday = (date) => {
    const day = date.getDay();
    const today = startOfDay(new Date());
    return day !== 0 && day !== 6 && !isBefore(date, today);
  };

  const getHungarianDayName = (date) => {
    const days = ['Vasárnap', 'Hétfő', 'Kedd', 'Szerda', 'Csütörtök', 'Péntek', 'Szombat'];
    return days[date.getDay()];
  };

  const calculateRequiredSlots = (startTime, serviceDuration) => {
    const slots = [];
    const slotCount = Math.ceil(serviceDuration / workingHours.interval);

    for (let i = 0; i < slotCount; i++) {
      const slotTime = new Date(startTime);
      slotTime.setMinutes(slotTime.getMinutes() + (i * workingHours.interval));
      slots.push(slotTime);
    }

    return slots;
  };

  const generateTimeSlots = (date) => {
    if (!date) return [];

    const slots = [];
    const now = new Date();
    const startTime = new Date(date);
    startTime.setHours(workingHours.start, 0, 0);

    const endTime = new Date(date);
    endTime.setHours(workingHours.end, 0, 0);

    while (startTime < endTime) {
      if (startTime > now) {
        slots.push(new Date(startTime));
      }
      startTime.setMinutes(startTime.getMinutes() + workingHours.interval);
    }

    return slots;
  };

  const isTimeSlotAvailable = (slot, selectedService) => {
    if (!selectedService) return true;

    const serviceDuration = servicesDurations[selectedService];
    const requiredSlots = calculateRequiredSlots(slot, serviceDuration);

    const lastSlot = requiredSlots[requiredSlots.length - 1];
    const workingEndTime = new Date(slot);
    workingEndTime.setHours(workingHours.end, 0, 0);

    if (lastSlot > workingEndTime) {
      return false;
    }

    return !requiredSlots.some(requiredSlot => {
      const requiredSlotTimeStr = `${requiredSlot.getHours().toString().padStart(2, '0')}:${requiredSlot.getMinutes().toString().padStart(2, '0')}`;

      return allAppointments.some(app => {
        if (!app.Datum) return false;

        try {
          const appDate = new Date(app.Datum);
          if (isNaN(appDate.getTime())) return false;

          if (appDate.toDateString() !== requiredSlot.toDateString()) return false;

          const appTimeStr = `${appDate.getHours().toString().padStart(2, '0')}:${appDate.getMinutes().toString().padStart(2, '0')}`;
          return appTimeStr === requiredSlotTimeStr;
        } catch (e) {
          console.error("Hiba az időpont ellenőrzése közben:", app.Datum, e);
          return false;
        }
      });
    });
  };

  const createAppointmentEntries = (mainAppointment, service) => {
    const appointments = [];
    const serviceDuration = servicesDurations[service];
    const slotCount = Math.ceil(serviceDuration / workingHours.interval);

    for (let i = 0; i < slotCount; i++) {
      const appointmentDate = new Date(mainAppointment.Datum);
      appointmentDate.setMinutes(appointmentDate.getMinutes() + (i * workingHours.interval));

      appointments.push({
        ...mainAppointment,
        Datum: appointmentDate.toISOString(),
        Szolgaltatas: i === 0 ? service : `${service} (folytatás)`
      });
    }

    return appointments;
  };

  useEffect(() => {
    if (formData.date) {
      const allSlots = generateTimeSlots(formData.date);

      const available = allSlots.filter(slot =>
        isTimeSlotAvailable(slot, formData.service)
      );

      setAvailableTimes(available);
    } else {
      setAvailableTimes([]);
    }
  }, [formData.date, formData.service, allAppointments]);

  const handleMakeChange = (e) => {
    const value = e.target.value;
    setFormData(prev => ({ ...prev, make: value, model: '' }));
    setModelSuggestions([]);
    setShowModelSuggestions(false);

    if (value.trim().length > 0) {
      const filteredMakes = allMakes.filter(make =>
        make.toLowerCase().includes(value.toLowerCase())
      );
      setMakeSuggestions(filteredMakes);
      setShowMakeSuggestions(true);
    } else {
      setMakeSuggestions([]);
      setShowMakeSuggestions(false);
    }

    if (errors.make) {
      setErrors(prev => ({ ...prev, make: undefined }));
    }
  };

  const handleMakeSuggestionClick = (make) => {
    setFormData(prev => ({ ...prev, make: make, model: '' }));
    setMakeSuggestions([]);
    setShowMakeSuggestions(false);
    setModelSuggestions([]);
    setShowModelSuggestions(false);

    if (errors.make) {
      setErrors(prev => ({ ...prev, make: undefined }));
    }
    document.getElementById('model')?.focus();
  };

  const handleModelChange = (e) => {
    const value = e.target.value;
    setFormData(prev => ({ ...prev, model: value }));

    const currentMake = formData.make;
    if (currentMake && carData[currentMake]) {
      if (value.trim().length > 0) {
        const filteredModels = carData[currentMake].filter(model =>
          model.toLowerCase().includes(value.toLowerCase())
        );
        setModelSuggestions(filteredModels);
        setShowModelSuggestions(true);
      } else {
        const allModelsForMake = [...carData[currentMake]];

        setModelSuggestions(allModelsForMake);
        setShowModelSuggestions(!!value);
      }
    } else {
      setModelSuggestions([]);
      setShowModelSuggestions(false);
    }
    if (errors.model) {
      setErrors(prev => ({ ...prev, model: undefined }));
    }
  };

  const handleModelSuggestionClick = (model) => {
    setFormData(prev => ({ ...prev, model: model }));
    setModelSuggestions([]);
    setShowModelSuggestions(false);
    if (errors.model) {
      setErrors(prev => ({ ...prev, model: undefined }));
    }
    document.getElementById('year')?.focus();
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    if (name === 'make' || name === 'model') return;

    if (name === 'service') {
      setFormData(prev => ({ ...prev, [name]: value, time: '' }));
    } else {
      setFormData(prev => ({ ...prev, [name]: value }));
    }

    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: undefined }));
    }
  };

  const validateForm = () => {
    const newErrors = {};
    const currentYear = new Date().getFullYear();

    if (!formData.service) newErrors.service = 'Válasszon szolgáltatást';
    if (!formData.date) newErrors.date = 'Válasszon dátumot';
    if (!formData.time) newErrors.time = 'Válasszon időpontot';

    if (!formData.name.trim()) newErrors.name = 'Adja meg a nevét';
    if (!/^\S+@\S+\.\S+$/.test(formData.Email_cim)) newErrors.Email_cim = 'Érvénytelen email cím';
    if (!/^\+?[0-9\s-]{9,}$/.test(formData.phone)) newErrors.phone = 'Érvénytelen telefonszám';
    if (!formData.address.trim()) newErrors.address = 'Adja meg a lakcímét';

    if (!formData.licensePlate.trim()) newErrors.licensePlate = 'Adja meg a rendszámot';
    if (!formData.vin.trim()) newErrors.vin = 'Adja meg az alvázszámot';

    if (!formData.make.trim()) {
      newErrors.make = 'Adja meg a márkát';
    }

    if (!formData.model.trim()) {
      newErrors.model = 'Adja meg a modellt';
    }

    if (!formData.year || isNaN(formData.year) || formData.year > currentYear || formData.year < 1900)
      newErrors.year = `Érvénytelen év (1900-${currentYear})`;
    if (formData.mileage === '' || isNaN(formData.mileage) || formData.mileage < 0)
      newErrors.mileage = 'Érvénytelen km állás (nem lehet negatív)';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const hideSuggestions = (delay = 150) => {
    setTimeout(() => {
      setShowMakeSuggestions(false);
      setShowModelSuggestions(false);
    }, delay);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    const appointmentDateTime = new Date(formData.date);
    const [hours, minutes] = formData.time.split(':');
    appointmentDateTime.setHours(parseInt(hours, 10), parseInt(minutes, 10));

    const now = new Date();
    if (appointmentDateTime < now) {
      setErrors(prev => ({ ...prev, time: 'A kiválasztott időpont a múltban van.' }));
      return;
    }

    const utcDate = new Date(
      Date.UTC(
        appointmentDateTime.getFullYear(), appointmentDateTime.getMonth(), appointmentDateTime.getDate(),
        appointmentDateTime.getHours(), appointmentDateTime.getMinutes()
      )
    );


    const mainAppointmentData = {
      Datum: utcDate.toISOString(),
      Nap: getHungarianDayName(formData.date),
      Statusz: 'Foglalt',
    };

    const costumerData = {
      Nev: formData.name,
      Email_cim: formData.Email_cim,
      Telefonszam: formData.phone,
      Lakcim: formData.address,
    };

    try {
      const appointmentEntries = createAppointmentEntries(mainAppointmentData, formData.service);

      const createdAppointmentIds = [];
      for (const appointmentData of appointmentEntries) {
        const resp = await fetch('https://localhost:44364/api/Idopont', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(appointmentData),
        });
        if (!resp.ok) throw new Error('Időpont létrehozása sikertelen');
        const saved = await resp.json();
        createdAppointmentIds.push(saved.Idopont_id);
      }

      const customerResp = await fetch('https://localhost:44364/api/Ugyfelek', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(costumerData),
      });
      if (!customerResp.ok) throw new Error('Ügyfél létrehozása sikertelen');
      const newCustomer = await customerResp.json();
      const ügyfelId = newCustomer.Ugyfel_id;

      for (const idopontId of createdAppointmentIds) {
        const foglalData = { Idopont_id: idopontId, Ugyfel_id: ügyfelId };
        const foglalResp = await fetch('https://localhost:44364/api/Foglal', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(foglalData),
        });
        if (!foglalResp.ok) {
          console.warn(`Foglalás sikertelen Idopont_id=${idopontId}`);
        }
      }

      const vehicleData = {
        Rendszam: formData.licensePlate,
        Alvazszam: formData.vin,
        Marka: formData.make,
        Modell: formData.model,
        Gyartasi_ev: parseInt(formData.year, 10),
        Km_ora_allas: parseInt(formData.mileage, 10),
        Ugyfel_id: newCustomer.Ugyfel_id
      };

      try {
        await fetch('https://localhost:44364/api/Jarmuvek', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json'
          },
          body: JSON.stringify(vehicleData),
        });
      } catch (vehicleError) {
        console.warn('Vehicle data could not be saved:', vehicleError);
      }

      const serviceId = 'service_7sfvd76';
      const templateId = 'template_tcxid7i';
      const publicKey = 'dm3B_05FO4pRSsO3X';

      const templateParams = {
        to_name: formData.name,
        from_name: 'Autószerviz',
        service_name: formData.service,
        booking_date: formData.date ? formData.date.toLocaleDateString('hu-HU') : '',
        booking_time: formData.time,
        customer_email: formData.Email_cim,
        customer_phone: formData.phone,
        car_license: formData.licensePlate,
        car_vin: formData.vin,
        car_make: formData.make,
        car_model: formData.model,
        car_year: formData.year,
        car_mileage: formData.mileage,
        duration: formData.duration
      };

      try {
        await emailjs.send(serviceId, templateId, templateParams, publicKey);
        console.log('Email sikeresen elküldve!');
      } catch (emailError) {
        console.error('Hiba az email küldése közben:', emailError);
      }


      const newAppointments = await fetchAppointments();
      setAllAppointments(newAppointments);
      setIsSubmitted(true);
      setFormData({
        service: '', date: null, time: '', name: '', Email_cim: '', phone: '', address: '', licensePlate: '', vin: '', make: '', model: '', year: '', mileage: ''
      });
    } catch (error) {
      console.error('Hiba:', error);
      alert(`Hiba történt a foglalás során! Részletek: ${error.message}`);
    }
  };

  const handleDateChange = (date) => {
    setFormData(prev => ({ ...prev, date: date, time: '' }));
    setErrors(prev => ({ ...prev, date: undefined, time: undefined }));
  };

  const getServiceDurationDisplay = (service) => {
    if (!service) return "";
    const duration = servicesDurations[service];
    const hours = Math.floor(duration / 60);
    const minutes = duration % 60;

    if (hours > 0 && minutes > 0) {
      return `(${hours} óra ${minutes} perc)`;
    } else if (hours > 0) {
      return `(${hours} óra)`;
    } else {
      return `(${minutes} perc)`;
    }
  };

  if (isSubmitted) {
    return (
      <div className={styles.confirmation}>
        <h2>Köszönjük foglalását!</h2>
        <p>A foglalás részleteiről hamarosan visszaigazolást küldünk.</p>
        <button onClick={() => setIsSubmitted(false)} className={styles.submitButton} style={{ marginTop: '20px', backgroundColor: '#007bff' }}>Új foglalás</button>
      </div>
    );
  }

  return (
    <div className={styles.container}>
      <h1>Időpontfoglalás</h1>
      {!isSubmitted && (
        <form onSubmit={handleSubmit} className={styles.form}>
          <div className={styles.formColumns}>

            <div className={styles.column}>
              <h2>Időpont</h2>
              <div className={styles.formGroup}>
                <label htmlFor="service">Szolgáltatás:</label>
                <select
                  id="service"
                  name="service"
                  value={formData.service}
                  onChange={handleChange}
                  className={errors.service ? styles.error : ''}
                  aria-invalid={!!errors.service}
                  aria-describedby={errors.service ? "service-error" : undefined}
                >
                  <option value="">-- Válasszon --</option>
                  {services.map((service) => (
                    <option key={service} value={service}>
                      {service} {getServiceDurationDisplay(service)}
                    </option>
                  ))}
                </select>
                {errors.service && <span id="service-error" className={styles.errorMsg}>{errors.service}</span>}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="date-picker">Dátum:</label>
                <DatePicker
                  locale="hu"
                  id="date-picker"
                  selected={formData.date}
                  onChange={handleDateChange}
                  minDate={new Date()}
                  filterDate={isWeekday}
                  dateFormat="yyyy/MM/dd"
                  className={`${styles.datePickerInput} ${errors.date ? styles.error : ''}`}
                  placeholderText="Válasszon napot"
                  aria-required="true"
                  aria-invalid={!!errors.date}
                  aria-describedby={errors.date ? "date-error" : undefined}
                  autoComplete="off"
                />
                {errors.date && <span id="date-error" className={styles.errorMsg}>{errors.date}</span>}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="time">Időpont:</label>
                <select
                  id="time"
                  name="time"
                  value={formData.time}
                  onChange={handleChange}
                  className={errors.time ? styles.error : ''}
                  disabled={!formData.date || !formData.service || availableTimes.length === 0}
                  aria-required="true"
                  aria-invalid={!!errors.time}
                  aria-describedby={errors.time ? "time-error" : undefined}
                >
                  <option value="">
                    {!formData.date
                      ? 'Előbb válasszon dátumot'
                      : !formData.service
                        ? 'Előbb válasszon szolgáltatást'
                        : availableTimes.length > 0
                          ? '-- Válasszon --'
                          : 'Nincs szabad időpont'}
                  </option>
                  {availableTimes.map((time, index) => (
                    <option key={index} value={time.toTimeString().slice(0, 5)}>
                      {time.toLocaleTimeString('hu-HU', { hour: '2-digit', minute: '2-digit' })}
                    </option>
                  ))}
                </select>
                {errors.time && <span id="time-error" className={styles.errorMsg}>{errors.time}</span>}
                {formData.service && formData.time && (
                  <span className={styles.serviceDuration}>
                    Időtartam: {getServiceDurationDisplay(formData.service).replace(/[()]/g, '')}
                  </span>
                )}
              </div>
            </div>

            <div className={styles.column}>
              <h2>Személyes adatok</h2>
              <div className={styles.formGroup}>
                <label htmlFor="name">Név:</label>
                <input
                  type="text"
                  id="name"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                  className={errors.name ? styles.error : ''}
                  placeholder="Kiss János"
                  aria-required="true"
                  aria-invalid={!!errors.name}
                  aria-describedby={errors.name ? "name-error" : undefined}
                  autoComplete="name"
                />
                {errors.name && <span id="name-error" className={styles.errorMsg}>{errors.name}</span>}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="Email_cim">Email:</label>
                <input
                  type="email"
                  id="Email_cim"
                  name="Email_cim"
                  value={formData.Email_cim}
                  onChange={handleChange}
                  className={errors.Email_cim ? styles.error : ''}
                  placeholder="pelda@email.hu"
                  aria-required="true"
                  aria-invalid={!!errors.Email_cim}
                  aria-describedby={errors.Email_cim ? "email-error" : undefined}
                  autoComplete="email"
                />
                {errors.Email_cim && <span id="email-error" className={styles.errorMsg}>{errors.Email_cim}</span>}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="phone">Telefonszám:</label>
                <input
                  type="tel"
                  id="phone"
                  name="phone"
                  value={formData.phone}
                  onChange={handleChange}
                  className={errors.phone ? styles.error : ''}
                  placeholder="+36 20 123 4567"
                  aria-required="true"
                  aria-invalid={!!errors.phone}
                  aria-describedby={errors.phone ? "phone-error" : undefined}
                  autoComplete="tel"
                />
                {errors.phone && <span id="phone-error" className={styles.errorMsg}>{errors.phone}</span>}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="address">Lakcím:</label>
                <input
                  type="text"
                  id="address"
                  name="address"
                  value={formData.address}
                  onChange={handleChange}
                  className={errors.address ? styles.error : ''}
                  placeholder="9700 Szombathely, Fő utca 1."
                  aria-required="true"
                  aria-invalid={!!errors.address}
                  aria-describedby={errors.address ? "address-error" : undefined}
                  autoComplete="street-address"
                />
                {errors.address && <span id="address-error" className={styles.errorMsg}>{errors.address}</span>}
              </div>
            </div>

            <div className={styles.column}>
              <h2>Autó adatai</h2>
              <div className={styles.formGroup}>
                <label htmlFor="licensePlate">Rendszám:</label>
                <input
                  type="text"
                  id="licensePlate"
                  name="licensePlate"
                  value={formData.licensePlate}
                  onChange={handleChange}
                  className={errors.licensePlate ? styles.error : ''}
                  placeholder="AABC-123"
                  aria-required="true"
                  aria-invalid={!!errors.licensePlate}
                  aria-describedby={errors.licensePlate ? "licensePlate-error" : undefined}
                  style={{ textTransform: 'uppercase' }}
                />
                {errors.licensePlate && <span id="licensePlate-error" className={styles.errorMsg}>{errors.licensePlate}</span>}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="vin">Alvázszám (VIN):</label>
                <input
                  type="text"
                  id="vin"
                  name="vin"
                  value={formData.vin}
                  onChange={handleChange}
                  className={errors.vin ? styles.error : ''}
                  placeholder="17 karakter"
                  maxLength="17"
                  aria-required="true"
                  aria-invalid={!!errors.vin}
                  aria-describedby={errors.vin ? "vin-error" : undefined}
                  style={{ textTransform: 'uppercase' }}
                />
                {errors.vin && <span id="vin-error" className={styles.errorMsg}>{errors.vin}</span>}
              </div>

              <div className={`${styles.formGroup} ${styles.autocompleteContainer}`}>
                <label htmlFor="make">Márka:</label>
                <input
                  type="text"
                  id="make"
                  name="make"
                  value={formData.make}
                  onChange={handleMakeChange}
                  onBlur={() => hideSuggestions()}
                  className={errors.make ? styles.error : ''}
                  placeholder="Kezdje el gépelni"
                  aria-required="true"
                  aria-invalid={!!errors.make}
                  aria-describedby={errors.make ? "make-error" : undefined}
                  autoComplete="off"
                />
                {errors.make && <span id="make-error" className={styles.errorMsg}>{errors.make}</span>}
                {showMakeSuggestions && makeSuggestions.length > 0 && (
                  <ul className={styles.suggestionsList}>
                    {makeSuggestions.map((make) => (
                      <li key={make} onMouseDown={(e) => {
                        e.preventDefault();
                        handleMakeSuggestionClick(make);
                      }}>
                        {make}
                      </li>
                    ))}
                  </ul>
                )}
              </div>

              <div className={`${styles.formGroup} ${styles.autocompleteContainer}`}>
                <label htmlFor="model">Modell:</label>
                <input
                  type="text"
                  id="model"
                  name="model"
                  value={formData.model}
                  onChange={handleModelChange}
                  onBlur={() => hideSuggestions()}
                  className={errors.model ? styles.error : ''}
                  placeholder={formData.make ? 'Kezdje el gépelni' : 'Előbb válasszon márkát'}
                  aria-required="true"
                  aria-invalid={!!errors.model}
                  aria-describedby={errors.model ? "model-error" : undefined}
                  disabled={!formData.make}
                  autoComplete="off"
                />
                {errors.model && <span id="model-error" className={styles.errorMsg}>{errors.model}</span>}

                {showModelSuggestions && modelSuggestions.length > 0 && carData[formData.make] && (
                  <ul className={styles.suggestionsList}>
                    {modelSuggestions.map((model) => (
                      <li key={model} onMouseDown={(e) => {
                        e.preventDefault();
                        handleModelSuggestionClick(model);
                      }}>
                        {model}
                      </li>
                    ))}
                  </ul>
                )}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="year">Gyártási év:</label>
                <input
                  type="number"
                  id="year"
                  name="year"
                  value={formData.year}
                  onChange={handleChange}
                  className={errors.year ? styles.error : ''}
                  placeholder="2015"
                  min="1900"
                  max={new Date().getFullYear()}
                  aria-required="true"
                  aria-invalid={!!errors.year}
                  aria-describedby={errors.year ? "year-error" : undefined}
                />
                {errors.year && <span id="year-error" className={styles.errorMsg}>{errors.year}</span>}
              </div>

              <div className={styles.formGroup}>
                <label htmlFor="mileage">Km óra állás:</label>
                <input
                  type="number"
                  id="mileage"
                  name="mileage"
                  value={formData.mileage}
                  onChange={handleChange}
                  className={errors.mileage ? styles.error : ''}
                  placeholder="150000"
                  min="0"
                  aria-required="true"
                  aria-invalid={!!errors.mileage}
                  aria-describedby={errors.mileage ? "mileage-error" : undefined}
                />
                {errors.mileage && <span id="mileage-error" className={styles.errorMsg}>{errors.mileage}</span>}
              </div>
            </div>

          </div>

          {errors.form && (
            <div className={`${styles.formGroup} ${styles.fullWidthError}`}>
              <span className={styles.errorMsg}>{errors.form}</span>
            </div>
          )}

          <div className={styles.submitButtonContainer}>
            <button type="submit" className={styles.submitButton}>Foglalás</button>
          </div>
        </form>
      )}
    </div>
  );
};

export default Booking;
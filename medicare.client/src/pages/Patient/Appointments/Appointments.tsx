import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import DatePicker from "react-datepicker";
import { FaCalendarAlt } from "react-icons/fa";
import "react-datepicker/dist/react-datepicker.css";
import "bootstrap/dist/css/bootstrap.min.css";
import "./Appointments.css";
import { useAuth } from "../../../context/AuthContext";
import { getSpecializationNames } from "../../../services/specializationsService";
import { getDoctorsBySpecialization } from "../../../services/doctorsService";
import { getVisitsTime } from "../../../services/visitsService";
import { getVisitsRoom } from "../../../services/visitsService";
import { getPatientMe } from "../../../services/patientsService";

const BookingPage = () => {
    interface SpecializationsNamesID {
        id: number;
        specializationName: string;
    }
    interface DoctorApointmentsDto {
        id: number;
        name: string;
        surname: string;
        startHour: string;
        endHour: string;
    }
    interface VisitTimeDto {
        visitTime: string;
        room: string;
    }
    interface Patient {
        id: number;
        name: string;
        surname: string;
        pesel: string;
        email: string;
        phoneNumber: string;
    }
    interface RoomDto {
        roomType: string;
        roomNumber: string;
    }

    // data
    const [specializations, setSpecializations] = useState<SpecializationsNamesID[]>([]);
    const [doctors, setDoctors] = useState<DoctorApointmentsDto[]>([]);
    const [selectedSpecialization, setSelectedSpecialization] = useState("");
    const [selectedDate, setSelectedDate] = useState<Date | null>(null);
    const [bookedTimes, setBookedTimes] = useState<{ time: string, room: string }[]>([]);
    const [selectedRoom, setSelectedRoom] = useState<RoomDto | null>(null);

    // data form logged patient
    const [loggedPatient, setLoggedPatienet] = useState<Patient>();

    const [selectedReason, setSelectedReason] = useState<number>(1);
    const [selectedDoctor, setSelectedDoctor] = useState("");
    const [selectedSlot, setSelectedSlot] = useState("");

    const [additionalNotes, setAdditionalNotes] = useState("");
    const [termsAccepted, setTermsAccepted] = useState(false);

    const [loadingSpecs, setLoadingSpecs] = useState(true);
    const [loadingDoctors, setloadingDoctors] = useState(false);

    const {userRole} = useAuth();

    const visitReasons = [
        { id: 1, label: "Consultation" },
        { id: 2, label: "Follow-up" },
        { id: 3, label: "Prescription" },
        { id: 4, label: "Checkup" }
    ];

    // get specializations
    useEffect(() => {
        getSpecializationNames()
            .then(setSpecializations)
            .catch(console.error)
            .finally(() => setLoadingSpecs(false));
    }, []);

    // get get doctors info by specialization ID
    useEffect(() => {
        if (selectedSpecialization) {
            setloadingDoctors(true);

            getDoctorsBySpecialization(parseInt(selectedSpecialization, 10))
                .then(data => setDoctors(data))
                .catch(err => console.error(err))
                .finally(() => setloadingDoctors(false));
        }
    }, [selectedSpecialization]);

    // get available dates 
    useEffect(() => {
        if (selectedDoctor) {
            const doctorObj = doctors.find(d => (d.name + " " + d.surname) === selectedDoctor);

            if (!doctorObj || !selectedDate) return;

            getVisitsTime(doctorObj.id, selectedDate!)
                .then((data: VisitTimeDto[]) => {

                    const timesWithRooms = data.map(v => ({
                        time: v.visitTime.slice(0, 5),
                        room: v.room
                    }));

                    setBookedTimes(timesWithRooms);
                });
        }
    }, [selectedDoctor, selectedDate]);

    // get loged patient data
    useEffect(() => {
        if (userRole !== "Patient") return;

        getPatientMe()
            .then(data => setLoggedPatienet(data))
            .catch(err => {
                console.error("Not logged in or error fetching patient:", err);
                setLoggedPatienet(undefined);
            });
    }, [userRole]);

    useEffect(() => {
        if (!selectedSpecialization) {
            setSelectedRoom(null);
            return;
        }

        getVisitsRoom(Number(selectedSpecialization))
            .then(data => setSelectedRoom(data))
            .catch(err => console.error(err));
    }, [selectedSpecialization]);

    const getSpecializationByID = () => {
        const spec = specializations.find(s => s.id == parseInt(selectedSpecialization, 10));
        return spec ? spec.specializationName : "";
    }

    function generateTimeSlots(start: string, end: string, intervalMinutes = 30): string[] {
        const slots: string[] = [];
        const [startH, startM] = start.split(":").map(Number);
        const [endH, endM] = end.split(":").map(Number);

        const current = new Date();
        current.setHours(startH, startM, 0, 0);

        const endTime = new Date();
        endTime.setHours(endH, endM, 0, 0);

        while (current <= endTime) {
            const hh = current.getHours().toString().padStart(2, "0");
            const mm = current.getMinutes().toString().padStart(2, "0");

            slots.push(`${hh}:${mm}`);
            current.setMinutes(current.getMinutes() + intervalMinutes);
        }

        return slots;
    }

    const selectedDocObj = doctors.find(d => (d.name + " " + d.surname) === selectedDoctor);

    const timeSlots = selectedDocObj && selectedDate
        ? generateTimeSlots(selectedDocObj.startHour, selectedDocObj.endHour)
            .filter(slot => !bookedTimes.some(b => b.time === slot))
        : [];

    const handleCreateVisit = async () => {
        if (!termsAccepted) {
            alert("You must accept the terms before booking.");
            return;
        }

        if (!selectedDocObj || !selectedDate || !selectedSlot || !loggedPatient) {
            alert("Please fill in all required fields.");
            return;
        }
        function formatDateLocal(date: Date): string {
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, "0");
            const day = String(date.getDate()).padStart(2, "0");
            return `${year}-${month}-${day}`;
        }

        const payload = {
            visitDate: formatDateLocal(selectedDate),
            visitTime: selectedSlot + ":00",
            doctorID: selectedDocObj.id,
            patientID: loggedPatient.id,
            reason: selectedReason,
            additionalNotes: additionalNotes
        };

        try {
            const response = await fetch("https://localhost:7014/api/visits", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(payload)
            });

            if (response.ok) {
                alert("Appointment successfully booked!");
            } else {
                const error = await response.text();
                alert("Error: " + error);
            }
        } catch (err) {
            console.error(err);
            alert("Unexpected error while booking appointment.");
        }
    };


    if (loadingSpecs || loadingDoctors) return <p>Loading...</p>

    return (
        <main className="container my-5 booking-page">
            {/* Header */}
            <div className="text-center mb-5">
                <h1 className="fw-bold text-primary">Book an appointment</h1>
                <p className="text-muted">Choose a doctor, date, and time to schedule your visit</p>
            </div>

            {!userRole && (
                <div className="alert alert-warning text-center">
                    You must be logged in to book an appointment.
                    <Link to="/login/patient" className="alert-link ms-2">Log in</Link>
                </div>
            )}

            <div className="row g-4">
                {/* Left: Booking form */}
                <div className="col-lg-8">
                    <div className="card shadow-sm p-4">
                        <h4 className="mb-4">Reservation form</h4>

                        {/* Specialization & Doctor */}
                        <div className="row">
                            <div className="col-md-6 mb-3">
                                <label className="form-label">Specialization</label>
                                <select
                                    className="form-select"
                                    value={selectedSpecialization}
                                    onChange={(e) => {
                                        setSelectedSpecialization(e.target.value);
                                        setSelectedDoctor("");
                                    }}
                                >
                                    <option value="">-- Choose specialization --</option>
                                    {specializations.map((spec) => (
                                        <option key={spec.id} value={spec.id}>
                                            {spec.specializationName}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="col-md-6 mb-3">
                                <label className="form-label">Select doctor</label>
                                <select
                                    className="form-select"
                                    value={selectedDoctor}
                                    onChange={(e) => setSelectedDoctor(e.target.value)}
                                    disabled={!selectedSpecialization}
                                >
                                    <option value="">
                                        {selectedSpecialization ? "-- Choose doctor --" : "Select specialization first"}
                                    </option>
                                    {doctors.map((doc) => (
                                        <option key={doc.id} value={doc.name + " " + doc.surname}>{doc.name} {doc.surname}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        {/* Date & time */}
                        <div className="col-4 mb-3">
                            <label className="form-label">Choose date</label>
                            <div className="input-group">
                                <DatePicker
                                    selected={selectedDate}
                                    onChange={(date: Date | null) => setSelectedDate(date)}
                                    filterDate={(date: Date) => {
                                        const day = date.getDay();
                                        return day !== 0 && day !== 6; // without saturday and sunday
                                    }}
                                    minDate={new Date(new Date().setDate(new Date().getDate() + 1))}
                                    dateFormat="yyyy-MM-dd"
                                    className="form-control"
                                />
                                <span className="input-group-text">
                                    <FaCalendarAlt />
                                </span>
                            </div>
                        </div>


                        {/* Time slots grid */}
                        <div className="mb-4">
                            <div className="d-flex align-items-center justify-content-between mb-2">
                                <h6 className="mb-0">Available time slots</h6>
                                <span className="badge bg-light text-dark">Monday &ndash; Friday</span>
                            </div>
                            <div className="timeslots-grid">
                                {timeSlots.map((t) => {
                                    const room = bookedTimes.find(b => b.time === t)?.room;
                                    return (
                                        <button
                                            key={t}
                                            type="button"
                                            className={`btn timeslot-btn ${selectedSlot === t ? "btn-primary" : "btn-outline-primary"}`}
                                            onClick={() => setSelectedSlot(t)}
                                        >
                                            {t} {room && <span className="badge bg-secondary ms-2">{room}</span>}
                                        </button>
                                    );
                                })}
                            </div>
                            <div className="form-text mt-2">Select a slot or use the time input above.</div>
                        </div>

                        {/* Patient info */}
                        <div className="row">
                            <div className="col-md-6 mb-3">
                                <label className="form-label">First name</label>
                                <p className="form-control-plaintext">{loggedPatient?.name}</p>
                            </div>
                            <div className="col-md-6 mb-3">
                                <label className="form-label">Last name</label>
                                <p className="form-control-plaintext">{loggedPatient?.surname}</p>
                            </div>
                        </div>

                        <div className="row">
                            <div className="col-md-6 mb-3">
                                <label className="form-label">Email</label>
                                <p className="form-control-plaintext">{loggedPatient?.email}</p>
                            </div>
                            <div className="col-md-6 mb-3">
                                <label className="form-label">Phone number</label>
                                <p className="form-control-plaintext">{loggedPatient?.phoneNumber}</p>
                            </div>
                        </div>

                        <div className="alert alert-info mt-2">
                            If any data is incorrect, check and modify it in your profile.
                        </div>


                        {/* Visit details */}
                        <div className="mb-3">
                            <label className="form-label">Visit reason</label>
                            <select className="form-select" value={selectedReason} onChange={e => setSelectedReason(Number(e.target.value))}>
                                {visitReasons.map(r => (
                                    <option key={r.id} value={r.id}>
                                        {r.label}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div className="mb-3">
                            <label className="form-label">Additional notes</label>
                            <textarea
                                className="form-control"
                                placeholder="Optional"
                                value={additionalNotes}
                                onChange={(e) => setAdditionalNotes(e.target.value)}
                            ></textarea>

                        </div>

                        {/* Terms & CTA */}
                        <div className="d-flex align-items-center justify-content-between">
                            <div className="form-check">
                                <input
                                    className="form-check-input"
                                    type="checkbox"
                                    id="terms"
                                    checked={termsAccepted}
                                    onChange={(e) => setTermsAccepted(e.target.checked)}
                                />

                                <label className="form-check-label" htmlFor="terms">
                                    I agree to the terms and privacy policy
                                </label>
                            </div>
                            <button
                                type="button"
                                className="btn btn-success btn-lg px-4"
                                onClick={handleCreateVisit}
                                disabled={!userRole}
                            >
                                Confirm appointment
                            </button>
                        </div>
                    </div>
                </div>

                {/* Right: Summary */}
                <div className="col-lg-4">
                    <div className="card shadow-sm p-4 mb-4">
                        <h5 className="mb-3">Appointment summary</h5>
                        <ul className="list-unstyled small">
                            <li><strong>Specialization:</strong> <span className="text-muted">{getSpecializationByID() || ""}</span></li>
                            <li><strong>Doctor:</strong> <span className="text-muted">{selectedDoctor || ""}</span></li>
                            <li><strong>Time slot:</strong> <span className="text-muted">{selectedSlot || ""}</span></li>
                            <li>
                                <strong>Room:</strong>{" "}
                                <span className="text-muted">
                                    {selectedRoom ? `${selectedRoom.roomType} (${selectedRoom.roomNumber})` : ""}
                                </span>
                            </li>

                        </ul>
                        <div className="alert alert-info mt-3 mb-0">
                            Please arrive 10 minutes before your scheduled time.
                        </div>
                    </div>

                    <div className="card shadow-sm p-4">
                        <h6 className="mb-2">Need help?</h6>
                        <p className="text-muted small mb-3">
                            If you can&rsquo;t find a suitable time, contact our reception. Some slots may open due to cancellations.
                        </p>
                        <Link to="/contact" className="btn btn-outline-primary w-100">Contact reception</Link>
                    </div>
                </div>
            </div>
        </main>
    );
};

export default BookingPage;

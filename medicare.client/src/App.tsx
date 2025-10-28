import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./layout/Layout";
import MinimalLayout from "./layout/MinimalLayout";
import PatientsPage from "./pages/Patient/PatientsPage";
import AboutUs from "./pages/Other/AboutUs/AboutUs";
import Contact from "./pages/Other/Contact/Contact";
import AllNews from "./pages/News/AllNews/AllNews";
import SelectedNews from "./pages/News/SelectedNews/SelectedNews";
import PatientLogin from "./pages/Patient/PatientLogin/PatientLogin";
import DoctorLogin from "./pages/Doctor/DoctorLogin/DoctorLogin";
import PatientRegister from "./pages/Patient/PatientRegister/PatientRegister";
import DoctorRegister from "./pages/Doctor/DoctorRegister/DoctorRegister";
import PatientProfile from "./pages/Patient/PatientProfile/PatientProfile";
import DoctorProfile from "./pages/Doctor/DoctorProfile/DoctorProfile";
import DoctorsList from "./pages/Doctor/DoctorsList/DoctorsList";
import DoctorInfo from "./pages/Doctor/DoctorInfo/DoctorInfo";
import DoctorVisits from "./pages/Doctor/DoctorVisits/DoctorVisits";
import Appointments from "./pages/Patient/Appointments/Appointments";
import DoctorAppointments from "./pages/Doctor/DoctorAppointments/DoctorAppointments";
import BookingSuccessPage from "./pages/Patient/BookingSuccessPage/BookingSuccessPage";
import DoctorsResetPassword from "./pages/Doctor/DoctorResetPassword/DoctorResetPassword";
import PatientsResetPassword from "./pages/Patient/PatientResetPassword/PatientResetPassword";
import { AuthProvider } from "./context/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";
import NotFoundPage from "./pages/Other/NotFoundPage/NotFoundPage";
import PatientVisits from "./pages/Patient/PatientVisits/PatientVisits";
import './App.css';

function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>01
                    <Route path="/" element={<Layout />}>
                        <Route path="patients" element={<PatientsPage />} />
                    </Route>

                    <Route element={<MinimalLayout />}>
                        <Route path="/aboutUs" element={<AboutUs />} />
                        <Route path="/contact" element={<Contact />} />
                        <Route path="/allNews" element={<AllNews />} />
                        <Route path="/selectedNews/:id" element={<SelectedNews />} />

                        <Route path="/login/patient" element={<PatientLogin />} />
                        <Route path="/login/doctor" element={<DoctorLogin />} />
                        <Route path="/register/patient" element={<PatientRegister />} />
                        <Route path="/register/doctor" element={<DoctorRegister />} />

                        <Route
                            path="/personalData"
                            element={
                                <ProtectedRoute role="Patient">
                                    <PatientProfile />
                                </ProtectedRoute>
                            }
                        />

                        <Route
                            path="/personalDataDoctor"
                            element={
                                <ProtectedRoute role="Doctor">
                                    <DoctorProfile />
                                </ProtectedRoute>
                            }
                        />

                        <Route path="/doctor/visits"
                            element={
                            <ProtectedRoute role="Doctor">
                                <DoctorVisits />
                            </ProtectedRoute>
                        } />

                        <Route path="/patient/visits" element={
                            <ProtectedRoute role="Patient">
                                <PatientVisits />
                            </ProtectedRoute>
                        }/>
                    <Route path="/doctors" element={<DoctorsList />} />
                    <Route path="/resetPasswordDoctor" element={<DoctorsResetPassword />} />
                    <Route path="/resetPasswordPatient" element={<PatientsResetPassword />} />
                    <Route path="/appointments" element={<Appointments />} />
                    <Route path="/doctorAppointments" element={<DoctorAppointments />} />
                    <Route path="/booking-success" element={<BookingSuccessPage />} />

                    <Route path="/doctorInfo/:id" element={<DoctorInfo />} />
                    <Route path="*" element={<NotFoundPage />} />
                </Route>
            </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}

export default App;
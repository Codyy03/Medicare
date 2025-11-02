import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./layout/Layout";
import MinimalLayout from "./layout/MinimalLayout";
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
import ServerErrorPage from "./pages/Other/ServerErrorPage/ServerErrorPage";
import UnauthorizedPage from "./pages/Other/UnauthorizedPage/UnauthorizedPage";
import PatientVisits from "./pages/Patient/PatientVisits/PatientVisits";
import AdminDashboard from "./pages/Admin/AdminDashboard/AdminDashboard";
import AdminDoctors from "./pages/Admin/AdminDoctors/AdminDoctors";
import DoctorEdit from "./pages/Admin/AdminDoctors/DoctorEdit";
import AdminDoctorCreate from "./pages/Admin/AdminDoctors/AdminDoctorCreate";
import './App.css';

function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>01
                    <Route path="/" element={<Layout />}>
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
                            path="/resetPasswordPatient"
                            element={
                                <ProtectedRoute role="Patient">
                                    <PatientsResetPassword />
                                </ProtectedRoute>
                            }
                        />

                        <Route path="/patient/visits" element={
                            <ProtectedRoute role="Patient">
                                <PatientVisits />
                            </ProtectedRoute>
                        } />

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

                        <Route path="/doctorAppointments"
                            element={
                                <ProtectedRoute role="Doctor">
                                    <DoctorAppointments />
                                </ProtectedRoute>
                            } />

                        <Route path="/resetPasswordDoctor"
                            element={
                                <ProtectedRoute role="Doctor">
                                    <DoctorsResetPassword />
                                </ProtectedRoute>
                            } />
                        <Route path="/patient/visits" element={
                            <ProtectedRoute role="Patient">
                                <PatientVisits />
                            </ProtectedRoute>
                        } />

                     <Route path="/doctors" element={<DoctorsList />} />
                    <Route path="/appointments" element={<Appointments />} />
                    <Route path="/booking-success" element={<BookingSuccessPage />} />

                    <Route path="/doctorInfo/:id" element={<DoctorInfo />} />
                    <Route path="*" element={<NotFoundPage />} />
                   <Route path="/unauthorized" element={<UnauthorizedPage />} />
                    <Route path="/server-error" element={<ServerErrorPage />} />
                        
                  <Route path="/admin" element={<AdminDashboard />} />
                  <Route path="/admin/doctors" element={<AdminDoctors />} />
                  <Route path="/admin/doctorEdit/:id" element={<DoctorEdit />} />
                  <Route path="/admin/adminDoctorCreate" element={<AdminDoctorCreate />} />
                        
                </Route>
            </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}

export default App;
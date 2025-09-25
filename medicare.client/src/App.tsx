import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./layout/Layout";
import MinimalLayout from "./layout/MinimalLayout";
import PatientsPage from "./pages/PatientsPage";
import ClientPage from "./pages/ClientPanel";
import AboutUs from "./pages/AboutUs/AboutUs";
import Contact from "./pages/Contact/Contact";
import AllNews from "./pages/AllNews/AllNews";
import SelectedNews from "./pages/SelectedNews/SelectedNews";
import PatientLogin from "./pages/PatientLogin/PatientLogin";
import DoctorLogin from "./pages/DoctorLogin/DoctorLogin";
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route path="patients" element={<PatientsPage />} />
                    <Route path="client" element={<ClientPage />} />
                </Route>

                <Route element={<MinimalLayout />}>
                    <Route path="/aboutUs" element={<AboutUs />} />
                    <Route path="/contact" element={<Contact />} />
                    <Route path="/allNews" element={<AllNews />} />
                    <Route path="/selectedNews/:id" element={<SelectedNews />} />

                    <Route path="/login/patient" element={<PatientLogin />} />
                    <Route path="/login/doctor" element={<DoctorLogin />} />
                </Route>
            </Routes>
        </BrowserRouter>

    );
}

export default App;
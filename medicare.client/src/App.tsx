import { BrowserRouter, Routes, Route } from "react-router-dom";
import Layout from "./layout/Layout";
import MinimalLayout from "./layout/MinimalLayout";
import PatientsPage from "./pages/PatientsPage";
import ClientPage from "./pages/ClientPanel";
import AboutUs from "./pages/AboutUs/AboutUs";
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Layout />}>
                    <Route path="patients" element={<PatientsPage />} />
                    <Route path="client" element={<ClientPage />} />
                </Route>
                <Route path="/aboutUs" element={<MinimalLayout><AboutUs /></MinimalLayout>} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
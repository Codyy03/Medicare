import { Outlet } from "react-router-dom";
import Header from "../components/Header";
import Footer from "../components/Footer";

export default function MinimalLayout() {
    return (
        <>
            <Header />
            <main className="container mt-4">
                <Outlet />
            </main>
            <Footer />
        </>
    );
}

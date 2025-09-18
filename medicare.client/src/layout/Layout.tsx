import { Outlet } from "react-router-dom";
import Header from "../components/Header";
import CarouselSection from "../components/CarouselSection";
import Tests from "../components/Tests";
import News from "../components/News";
import AboutUs from "../components/AbousUs";
import Footer from "../components/Footer";

export default function Layout() {
    return (
        <>
            <Header />
            <CarouselSection />
            <Tests />
            <News />
            <AboutUs />
            
            <main className="container mt-4">
                <Outlet />
            </main>
            <Footer />
        </>
    );
}

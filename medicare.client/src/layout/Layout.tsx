import { Outlet } from "react-router-dom";
import Header from "../components/Header/Header";
import CarouselSection from "../components/CarouselSection/CarouselSection";
import Tests from "../components/Tests/Tests";
import News from "../components/News/News";
import AboutUs from "../components/AbousUsSection/AbousUsSection";
import Footer from "../components/Footer/Footer";

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

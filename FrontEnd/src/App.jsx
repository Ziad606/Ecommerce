import { Routes, Route } from "react-router-dom";

import "./App.css";
import Home from "./pages/Home";
import Login from "./pages/login";
import Footer from "./componants/Footer";
import Contact from "./pages/Contact";
import Header from "./componants/Header";

function App() {
  return (
    <>
 <Header />
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/login" element={<Login />} />
        <Route path="/Contact" element={<Contact />} />
      </Routes>
      {/*  */}
      <Footer />
    </>
  );
}

export default App;

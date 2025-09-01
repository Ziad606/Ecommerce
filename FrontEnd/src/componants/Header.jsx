import { useState } from "react";
import { FaHeart, FaShoppingCart, FaSearch, FaBars, FaTimes } from "react-icons/fa";
import { Link } from "react-router-dom";

const Header = () => {
  const [isMobileMenuOpen, setMobileMenuOpen] = useState(false);

  return (
    <header className="fixed top-0 left-0 w-full h-[70px] flex items-center justify-between px-4 md:px-16 border-b border-gray-200 bg-white z-50">
      {/* Logo */}
      <div className="text-2xl font-bold text-black">Exclusive</div>

      {/* Navigation (desktop) */}
      <nav className="hidden md:flex">
        <ul className="flex gap-6">
          <li>
            <Link
              to="/"
              className="text-black font-medium hover:text-red-500 transition-colors duration-300"
            >
              Home
            </Link>
          </li>
          <li>
            <Link
              to="/contact"
              className="text-black font-medium hover:text-red-500 transition-colors duration-300"
            >
              Contact
            </Link>
          </li>
          <li>
            <Link
              to="/about"
              className="text-black font-medium hover:text-red-500 transition-colors duration-300"
            >
              About
            </Link>
          </li>
          <li>
            <Link
              to="/signup"
              className="text-black font-medium hover:text-red-500 transition-colors duration-300"
            >
              Sign Up
            </Link>
          </li>
        </ul>
      </nav>

      {/* Search + icons */}
      <div className="hidden md:flex items-center gap-4">
        <div className="flex items-center border border-gray-300 rounded-md px-3 py-1 gap-2 w-[250px]">
          <input
            type="text"
            placeholder="What are you looking for?"
            className="outline-none border-none flex-1 text-sm"
          />
          <FaSearch className="text-gray-500 cursor-pointer" />
        </div>
        <FaHeart className="cursor-pointer hover:text-red-500 transition-colors duration-300 text-lg" />
        <FaShoppingCart className="cursor-pointer hover:text-red-500 transition-colors duration-300 text-lg" />
      </div>

      {/* Mobile menu button */}
      <button
        className="md:hidden text-xl text-black"
        onClick={() => setMobileMenuOpen(!isMobileMenuOpen)}
      >
        {isMobileMenuOpen ? <FaTimes /> : <FaBars />}
      </button>

      {/* Mobile menu */}
      {isMobileMenuOpen && (
        <div className="absolute top-[70px] left-0 w-full bg-white border-t border-gray-200 flex flex-col items-center py-4 md:hidden">
          <ul className="flex flex-col gap-4">
            <li>
              <Link
                to="/"
                className="text-black font-medium hover:text-red-500 transition-colors duration-300"
                onClick={() => setMobileMenuOpen(false)}
              >
                Home
              </Link>
            </li>
            <li>
              <Link
                to="/contact"
                className="text-black font-medium hover:text-red-500 transition-colors duration-300"
                onClick={() => setMobileMenuOpen(false)}
              >
                Contact
              </Link>
            </li>
            <li>
              <Link
                to="/about"
                className="text-black font-medium hover:text-red-500 transition-colors duration-300"
                onClick={() => setMobileMenuOpen(false)}
              >
                About
              </Link>
            </li>
            <li>
              <Link
                to="/signup"
                className="text-black font-medium hover:text-red-500 transition-colors duration-300"
                onClick={() => setMobileMenuOpen(false)}
              >
                Sign Up
              </Link>
            </li>
          </ul>
          {/* Optional: mobile search */}
          <div className="flex items-center border border-gray-300 rounded-md px-3 py-1 gap-2 mt-4 w-[90%]">
            <input
              type="text"
              placeholder="Search..."
              className="outline-none border-none flex-1 text-sm"
            />
            <FaSearch className="text-gray-500 cursor-pointer" />
          </div>
        </div>
      )}
    </header>
  );
};

export default Header;

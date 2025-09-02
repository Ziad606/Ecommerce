import { Link } from "react-router-dom";
import qrImage from "../assets/QrCode.png";
// icons
import { RiArrowRightDoubleLine } from "react-icons/ri";
import { FaFacebook, FaTwitter, FaInstagram, FaLinkedin } from "react-icons/fa";

const Footer = () => {
  // account links

  const accountLinks = [
    { name: "My Account", path: "/Profile" },
    { name: "Login", path: "/Login" },
    { name: "Register", path: "/Register" },
    { name: "Cart", path: "/Cart" },
    { name: "Wishlist", path: "/Wishlist" },
    { name: "Shop", path: "/Shop" },
  ];
  
 // quick links 
  const quickLinks = [
  { name: "Privacy Policy", path: "/privacy-policy" },
  { name: "Terms & Conditions", path: "/terms" },
  { name: "FAQ", path: "/faq" },
  { name: "Contact", path: "/contact" },
];
  return (
    <footer className="bg-black text-white p-6 mx-auto mt-8">
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-8 mb-4">
        {/* 1 */}
        <div>
          <p className="text-lg font-bold">Exclusive</p>
          <p className="hover:text-blue-400">Subscribe</p>
          <p className="text-gray-400">Get 10% off for your First Order</p>
          <form className="flex items-center mt-2 border border-white rounded-md overflow-hidden w-fit">
            <input
              type="email"
              placeholder="Enter your email"
              className="p-2 rounded-l-md border-none outline-none text-white bg-black placeholder-white"
            />
            <button className="p-2 rounded-r-md flex items-center justify-center">
              <RiArrowRightDoubleLine color="white" />
            </button>
          </form>
        </div>
        {/* 2 */}

        <div className="flex flex-col gap-3 w-64">
          <p className="text-lg font-bold">Support</p>
          <p>111 Bijoy sarani, Dhaka, DH 1515, Bangladesh.</p>
          <a href="mailto:exclusive@gmail.com" className="hover:text-blue-400">
            exclusive@gmail.com
          </a>
          <a href="tel:+88015888889999" className="hover:text-blue-400">
            +88015-88888-9999
          </a>
        </div>
        {/* 3 */}
        <div>
          <p className="text-lg font-bold">Account</p>
          {accountLinks.map((link, index) => (
            <p key={index}>
              <Link to={link.path} className="hover:text-blue-400">
                {link.name}
              </Link>
            </p>
          ))}
        </div>
        {/* 4 */}
        <div>
          <p className="text-lg font-bold">Quick Link </p>
          {quickLinks.map( ( link, index) => (
            <p key={index}>
              <Link to={link.path} className="hover:text-blue-400">
                {link.name}
              </Link>
            </p>
          ))}
        </div>
        {/* 5 */}
        <div>
          <p className="text-lg font-bold">Dowenload App </p>
          <p className="text-gray-400">Save $3 with App New User Only</p>
          <div className="flex gap-2 mt-2 align-center">
            <img src={qrImage} alt="Download App QR Code" />
            <div className="flex flex-col gap-2">
              <img
                src="https://upload.wikimedia.org/wikipedia/commons/7/78/Google_Play_Store_badge_EN.svg"
                alt="Google Play"
                className="w-32 display-block"
              />

              <img
                src="https://developer.apple.com/assets/elements/badges/download-on-the-app-store.svg"
                alt="App Store"
                className="w-32 "
              />
            </div>
          </div>
          <div className="flex gap-4 justify-center items-center mt-4">
            <a
              href="https://facebook.com"
              target="_blank"
              rel="noopener noreferrer"
            >
              <FaFacebook className="hover:text-blue-400" />
            </a>
            <a
              href="https://twitter.com"
              target="_blank"
              rel="noopener noreferrer"
            >
              <FaTwitter className=" hover:text-blue-400" />
            </a>
            <a
              href="https://instagram.com"
              target="_blank"
              rel="noopener noreferrer"
            >
              <FaInstagram className="hover:text-blue-400" />
            </a>
            <a
              href="https://linkedin.com"
              target="_blank"
              rel="noopener noreferrer"
            >
              <FaLinkedin className=" hover:text-blue-400" />
            </a>
          </div>
        </div>
      </div>
      {/* Copyright */}
      <hr />
      <p className="text-center mt-4">
        &copy; 2025 Your Company. All rights reserved.
      </p>
    </footer>
  );
};

export default Footer;

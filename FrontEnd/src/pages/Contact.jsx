import React from 'react';
import { FaPhoneAlt } from 'react-icons/fa';
import { Link } from 'react-router-dom';
import { FaEnvelope } from "react-icons/fa";
const Contact = () => {
  return (
    <div className="max-w-7xl mx-auto px-4 py-8 mt-12">
      {/* Breadcrumb */}
<div className="flex gap-4 p-4">
      <Link to="/" className="text-gray-500 hover:text-black">
        Home / 
      </Link>
      <Link to="/contact" className="text-black">
        Contact
      </Link>
    </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        {/* Contact Info */}
        <div className="p-6 border rounded-lg shadow-md">
          <h2 className=" font-bold mb-4 flex items-center gap-3">
            <span className="bg-red-500  inline-block  rounded-full text-white p-2"> <FaPhoneAlt/></span>
            Call To Us
          </h2>
          <p className="mb-4">We are available 24/7, 7 days a week.</p>
          <p className="font-semibold">Phone: +880161112222</p>
          <hr className="my-6"/>
          <h2 className="text-xl font-bold mb-4 flex items-center gap-3">
            <span className="bg-red-500  inline-block  rounded-full text-white p-2"><FaEnvelope/> </span>
            Write To US
          </h2>
          <p className="mb-4">Fill out our form and we will contact you within 24 hours.</p>
          <p className="font-semibold">Emails: customer@exclusive.com</p>
          <p className="font-semibold">Emails: support@exclusive.com</p>
        </div>

        {/* Contact Form */}
        <div className="md:col-span-2 p-6 border rounded-lg shadow-md">
          <form>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-4">
              <input type="text" placeholder="Your Name *" className="p-3 bg-gray-100 rounded-md w-full" required />
              <input type="email" placeholder="Your Email *" className="p-3 bg-gray-100 rounded-md w-full" required />
              <input type="tel" placeholder="Your Phone *" className="p-3 bg-gray-100 rounded-md w-full" required />
            </div>
            <textarea placeholder="Your Message" rows="8" className="p-3 bg-gray-100 rounded-md w-full mb-4"></textarea>
            <div className="text-right">
              <button type="submit" className="bg-red-500 text-white px-8 py-3 rounded-md hover:bg-red-600">Send Message</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Contact;
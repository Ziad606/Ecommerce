import SaleCard from "./saleCard"; 
import React, { useState, useEffect, useRef } from "react";
import { RiArrowRightSLine, RiArrowLeftSLine } from "react-icons/ri"
import CountdownTimer from "./CountDown";
import {fetchAPI} from "../utils/FetchAPI";


const SalesSection = () => {    
  const [products, setProducts] = useState([]);
  const sliderRef = useRef(null);

  useEffect(() => {
    const getProducts = async () => {
      const data = await fetchAPI("https://fakestoreapi.com/products");
      if (data) setProducts(data);
    };
    getProducts();
  }, []);

//   scrolling 
  const scrollLeft = () => {
    sliderRef.current.scrollBy({ left: -300, behavior: "smooth" });
  };

  const scrollRight = () => {
    sliderRef.current.scrollBy({ left: 300, behavior: "smooth" });
  };

// timer 
const [targetDate, setTargetDate] = useState(null);
useEffect(() => {
  setTargetDate(new Date(Date.now() + 3 * 24 * 60 * 60 * 1000)); // 3 days from now
}, []);


  return (
    <div className="max-w-7xl mx-auto  py-8 ">

      <p className="text-lg text-[rgba(219,68,68,1)] flex items-center">
        <span className="bg-[rgba(219,68,68,1)] w-4 h-10 inline-block mr-2 rounded  mb-3"></span>
        Todays
      </p>
 {/* sales timer  */}
      <div className="flex items-center justify-between mb-4 flex-col md:flex-row ">
        <div className="flex items-center gap-8 flex-col md:flex-row mb-2">
          <h2 className="text-4xl ">Flash Sales</h2>
          <CountdownTimer targetDate={targetDate} />
        </div>

        {/* scrolling buttons */}
        <div className="flex gap-2">
          <button onClick={scrollLeft} className="bg-gray-200 p-2 rounded-full hover:bg-gray-300">
            <RiArrowLeftSLine />
          </button>
          <button onClick={scrollRight} className="bg-gray-200 p-2 rounded-full hover:bg-gray-300">
            <RiArrowRightSLine />
          </button>
        </div>
      </div>

   
      <div ref={sliderRef} className="flex gap-3 overflow-x-hidden scroll-smooth ">
        {products.map((p) => (
          <SaleCard key={p.id} product={p} />
        ))}
      </div>

     
      <div className="flex justify-center ">
        <button className="bg-red-500 text-white px-6 py-2 rounded hover:bg-red-600">
          View All Products
        </button>
      </div>
    </div>
  );
};

export default SalesSection;

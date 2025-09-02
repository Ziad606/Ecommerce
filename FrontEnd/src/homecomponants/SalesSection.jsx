import SaleCard from "../componants/saleCard.jsx";
import React, { useState, useEffect, useRef } from "react";
import { SliderButtons } from "../pages/Home.jsx";

import CountdownTimer from "../componants/CountDown.jsx";
import { useFetch } from "../utils/useFetch.js";

const SalesSection = () => {
  // Fetch products using the custom hook
  const sliderRef = useRef(null);
  const { data: products, error } = useFetch(
    "https://fakestoreapi.com/products"
  );

  // timer
  const [targetDate, setTargetDate] = useState(null);
  useEffect(() => {
    setTargetDate(new Date(Date.now() + 3 * 24 * 60 * 60 * 1000)); // 3 days from now
  }, []);

  return (
    <div className="py-8 mt-10" id="saleproducts">
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
        <SliderButtons sliderRef={sliderRef} />
      </div>

      <div
        ref={sliderRef}
        className="flex gap-3 overflow-x-hidden scroll-smooth "
      >
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

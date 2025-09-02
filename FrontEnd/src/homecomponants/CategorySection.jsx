import {SliderButtons} from '../pages/Home.jsx'
import { useFetch } from "../utils/useFetch";
import { useRef } from "react";


const CategorySection = () => {
  // Fetch cat using the custom hook
  const sliderRef = useRef(null);
  const { data: products = [], error } = useFetch("https://fakestoreapi.com/products");

  // Extract unique categories
  const uniqueCategories = [...new Set(products.map(p => p.category))];
  return (
    <section className="py-8">
      <p className="text-lg text-[rgba(219,68,68,1)] flex items-center">
        <span className="bg-[rgba(219,68,68,1)] w-4 h-10 inline-block mr-2 rounded mb-3"></span>
        Our Categories
      </p>
 <div className="flex justify-between items-center mb-6 flex-col md:flex-row gap-4">
      <h2 className="text-4xl">Browse By Category</h2>
      <SliderButtons sliderRef={sliderRef}/> 
   </div> 
      <div ref={sliderRef} className="flex gap-3 overflow-x-auto scroll-smooth no-scrollbar">
        {uniqueCategories.map((cat) => (
          <div
            key={cat}
            className="border rounded-lg p-6 flex flex-col items-center justify-center cursor-pointer transition text-red-500 w-[170px] h-[145px] flex-shrink-0 mx-auto"
          >
            <p className="font-medium">{cat.toUpperCase()}</p>
          </div>
        ))}
      </div>

      {error && <p className="text-red-500 mt-4">Error fetching categories</p>}
    </section>
  );
};

export default CategorySection;

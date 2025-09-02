import { SliderButtons } from "../pages/Home.jsx";
import { useFetch } from "../utils/useFetch.js";
import { useRef } from "react";
import Card from "../componants/Card.jsx";
const PRoductsSection = () => {
  const sliderRef = useRef(null);
  const { data: products, error } = useFetch(
    "https://fakestoreapi.com/products"
  );
  //error
  if (error) return <p>Error loading products...</p>;
  if (!products) return <p>Loading...</p>;

  const chunkedProducts = [];
  for (let i = 0; i < products.length; i += 8) {
    chunkedProducts.push(products.slice(i, i + 8));
  }

  return (
    <div className="mx-auto max-w-[90%] lg:max-w-[1400px] py-8 ">
      <p className="text-lg text-[rgba(219,68,68,1)] flex items-center">
        <span className="bg-[rgba(219,68,68,1)] w-4 h-10 inline-block mr-2 rounded  mb-3"></span>
        OUr Products
      </p>

      <div className="flex justify-between items-center mb-6 flex-col md:flex-row gap-4">
        <h2 className="text-4xl">Explore Our Products </h2>
        <SliderButtons sliderRef={sliderRef} />
      </div>
      <div
        ref={sliderRef}
        className="overflow-x-auto scroll-smooth no-scrollbar"
      >
        <div className="flex gap-8">
          {chunkedProducts.map((group, index) => (
            <div
              key={index}
              className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 min-w-full"
            >
              {group.map((p) => (
                <Card key={p.id} product={p} />
              ))}
            </div>
          ))}
        </div>
      </div>
      <div className="flex justify-center ">
        <button className="bg-red-500 text-white px-6 py-2 rounded hover:bg-red-600">
          View All Products
        </button>
      </div>
    </div>
  );
};
export default PRoductsSection;

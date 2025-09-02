import { useRef, useEffect } from "react";
import Card from "../componants/Card.jsx";
import { useFetch } from "../utils/useFetch.js";
import { Link } from "react-router-dom";

const ThisMonth = () => {
  const sliderRef = useRef(null);
  const { data: products = [], error } = useFetch(
    "https://fakestoreapi.com/products"
  );

  // Store 4 random products in a ref so they don't change on re-render
  const randomProductsRef = useRef([]);

  useEffect(() => {
    if (products.length > 0 && randomProductsRef.current.length === 0) {
      randomProductsRef.current = [...products]
        .sort(() => 0.5 - Math.random())
        .slice(0, 4);
    }
  }, [products]);

  return (
    <div className="mx-auto max-w-[90%] lg:max-w-[1400px] py-8">
      <p className="text-lg text-[rgba(219,68,68,1)] flex items-center">
        <span className="bg-[rgba(219,68,68,1)] w-4 h-10 inline-block mr-2 rounded mb-3"></span>
        This Month
      </p>

      <div className="flex justify-between items-center mb-6 flex-col md:flex-row gap-4">
        <h2 className="text-4xl">Best Selling Products</h2>
        <Link to="/products">
          <button className="bg-[rgba(219,68,68,1)] p-3 text-white w-52 rounded-lg">
            View All
          </button>
        </Link>
      </div>

      {error && <p className="text-red-500">Error fetching products</p>}

      <div
        ref={sliderRef}
        className="flex flex-col md:flex-row gap-6 overflow-x-auto scroll-smooth items-center md:items-start"
      >
        {randomProductsRef.current.map((product) => (
          <Card key={product.id} product={product} />
        ))}
      </div>
    </div>
  );
};

export default ThisMonth;

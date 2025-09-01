import { useState, useEffect, useRef } from "react";
import { RiArrowRightSLine, RiArrowLeftSLine } from "react-icons/ri";
// import { FaTshirt, FaLaptop, FaGem, FaFemale } from "react-icons/fa";

// export const categoryIcons = {
//   "men's clothing": <FaTshirt size={32} />,
//   "women's clothing": <FaFemale size={32} />,
//   electronics: <FaLaptop size={32} />,
//   jewelry: <FaGem size={32} />,
// };

const CategorySection = () => {
  const [categories, setCategories] = useState([]);
  const sliderRef = useRef(null);

  useEffect(() => {
    const getCategories = async () => {
      try {
        const res = await fetch("https://fakestoreapi.com/products/categories");
        if (!res.ok) {
          throw new Error(`Failed to fetch: ${res.status} ${res.statusText}`);
        }
        const data = await res.json();
        if (data) setCategories(data);
      } catch (error) {
        console.error(error);
      }
    };
    getCategories();
  }, []);

  const scrollLeft = () => {
    if (sliderRef.current) {
      sliderRef.current.scrollBy({ left: -300, behavior: "smooth" });
    }
  };

  const scrollRight = () => {
    if (sliderRef.current) {
      sliderRef.current.scrollBy({ left: 300, behavior: "smooth" });
    }
  };

  return (
    <section className="py-8">
      <p className="text-lg text-[rgba(219,68,68,1)] flex items-center">
        <span className="bg-[rgba(219,68,68,1)] w-4 h-10 inline-block mr-2 rounded mb-3"></span>
       Our  Categories
      </p>

      <div className="flex justify-between items-center mb-6 flex-col md:flex-row gap-4">
        <h2 className="text-4xl">Browse By Category</h2>
        <div className="flex gap-2 my-3">
          <button onClick={scrollLeft} className="bg-gray-200 p-2 rounded-full hover:bg-gray-300">
            <RiArrowLeftSLine />
          </button>
          <button onClick={scrollRight} className="bg-gray-200 p-2 rounded-full hover:bg-gray-300">
            <RiArrowRightSLine />
          </button>
        </div>
      </div>
 {/* card  */}
      <div
  ref={sliderRef}
  className="flex gap-3 overflow-x-auto scroll-smooth no-scrollbar"
>
  {categories.map((cat) => (
    <div
      key={cat}
      className="border rounded-lg p-6 flex flex-col items-center justify-center cursor-pointer transition  text-red-500 w-[170px] h-[145px] flex-shrink-0 mx-auto"
    >
     
      <p className="font-medium">{cat.toUpperCase()}</p>
    </div>
  ))}
</div>

    </section>
  );
};

export default CategorySection;
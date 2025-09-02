
import { Link } from "react-router-dom";
import { FaHeart, FaEye  } from "react-icons/fa"
import { RiHeartLine } from 'react-icons/ri';
import { FaShoppingCart } from 'react-icons/fa';
// stars 
  const renderStars = (rate) => {
    if (!rate) return null;
    const fullStars = Math.floor(rate);
    const halfStar = rate - fullStars >= 0.5 ? 1 : 0;
    const emptyStars = 5 - fullStars - halfStar;
    return (
      <>
        {"★".repeat(fullStars)}
        {halfStar ? "½" : ""}
        {"☆".repeat(emptyStars)}
      </>
    );
  };
  // card 
 const Card = ({ product }) => {
  return (
    <div className="w-64 flex-shrink-0 text-white rounded-xl overflow-hidden transition-transform duration-300 ">
      {/* Image Container */}
     <div className="relative group bg-gray-100 p-3 cursor-pointer">
    <button className="bg-white p-1 rounded-full text-black hover:text-red-500 absolute top-2 right-2">
  <FaHeart />
</button>
  <Link to="/">
    <button className="bg-white text-gray-800 p-1 rounded-full hover:bg-gray-200 absolute top-2 right-10 ">
      <FaEye />
    </button>
    </Link> 
  <img
    src={product?.image}
    alt={product?.title || "Product"}
    className="w-full h-40  "
  />

  {/* Add to Cart Button sliding up */}
  <Link to="/">
  <button className="absolute bottom-0 left-0 w-full bg-black text-white px-4 py-2 text-sm transform translate-y-full opacity-0 group-hover:translate-y-0 group-hover:opacity-100 transition-all duration-300">
       🛒Add To Cart
  </button>
  </Link>
</div>
 {/* Info */}
<div className="p-4">
  <h3 className="text-lg font-semibold text-black ">{product?.title}</h3>
  <div className="flex items-center gap-2 mt-2 text-sm">
    <span className="text-red-400 font-bold">${product?.price}</span>
    <span className="line-through text-gray-400">
      ${(product?.price * 1.2).toFixed(2)}
    </span>
  </div>
  <p className="mt-1 text-yellow-400 text-sm">
    {renderStars(product?.rating?.rate)} ({product?.rating?.count})
  </p>
</div>

    </div>
  );
};

export default Card;

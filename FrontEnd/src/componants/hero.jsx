import Himage from "../assets/imageHero.png";
import "../css/hero.css";
export default function Hero() {
  return (
    <section className="bg-gray-50 pt-[50px] h-screen flex flex-col md:flex-row justify-around items-center gap-8 px-4 md:px-8 lg:px-16 relative overflow-hidden">
      {/* Left Text */}
      <div className="w-full md:w-1/2 text-animate text-center md:text-left">
        <p className="text-4xl sm:text-5xl md:text-6xl lg:text-7xl font-bold">
          Where Quality Meets{" "}
          <span className="text-red-500">Affordability</span>
        </p>
        <p className="text-sm sm:text-base md:text-lg py-5">
          Discover trending items, unbeatable deals, and a shopping experience
          designed just for you
        </p>
        <a href="#saleproducts">
          <button className="bg-red-500 text-white px-6 py-3 rounded text-base sm:text-lg transform transition-transform duration-200 active:scale-95">
            Shop Now
          </button>
        </a>
      </div>

      {/* Right Image */}
      <div className="relative flex justify-center items-center w-full md:w-1/2 mt-6 md:mt-0">
        <img
          src={Himage}
          alt="Featured Product"
          className="w-2/3 sm:w-1/2 md:w-4/5 lg:w-[600px] rounded-2xl z-10 image-animate"
        />

        {/* Red Divs */}
        <div className="absolute -top-8 -left-2 w-24 sm:w-32 md:w-40 h-24 sm:h-32 md:h-40 bg-red-500 rounded-full z-0 red-div-animate"></div>
        <div className="absolute -bottom-8 -right-6 w-32 sm:w-40 md:w-48 h-32 sm:h-40 md:h-48 bg-red-500 rounded-full z-0 red-div-animate"></div>
      </div>
    </section>
  );
}

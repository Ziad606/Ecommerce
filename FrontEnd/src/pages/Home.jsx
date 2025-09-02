import Hero from "../componants/hero";
import SalesSection from "../componants/SalesSection";
import CategorySection from "../componants/CategorySection";

import ThisMonth from "../homecomponants/thismonth";
import Black from "../homecomponants/black";
import PRoductsSection from "../homecomponants/OurProducts";
import Features from "../homecomponants/Features";
import { RiArrowRightSLine, RiArrowLeftSLine } from "react-icons/ri";

export function SliderButtons({ sliderRef }) {
  const scrollLeft = () =>
    sliderRef.current?.scrollBy({ left: -300, behavior: "smooth" });
  const scrollRight = () =>
    sliderRef.current?.scrollBy({ left: 300, behavior: "smooth" });

  return (
    <div className="flex gap-2 my-3">
      <button
        onClick={scrollLeft}
        className="bg-gray-200 p-2 rounded-full hover:bg-gray-300"
      >
        <RiArrowLeftSLine />
      </button>
      <button
        onClick={scrollRight}
        className="bg-gray-200 p-2 rounded-full hover:bg-gray-300"
      >
        <RiArrowRightSLine />
      </button>
    </div>
  );
}

const Home = () => {
  return (
    <>
      <Hero />
      <div className="mx-auto max-w-7xl">
        {/* sales section  */}
        <SalesSection />
        <hr />
        {/* category  */}
        <CategorySection />

        <hr />
        <ThisMonth />
      </div>
      <Black />
      <PRoductsSection />
      <Features />
    </>
  );
};
export default Home;

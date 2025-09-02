import { useEffect, useState } from "react";
import { RiArrowUpSLine } from "react-icons/ri"; 

const ScrollToTop = () => {
  const [isVisible, setIsVisible] = useState(false);


  useEffect(() => {
    const toggleVisibility = () => {
      if (window.scrollY > 300) {
        setIsVisible(true);
      } else {
        setIsVisible(false);
      }
    };

    window.addEventListener("scroll", toggleVisibility);
    return () => window.removeEventListener("scroll", toggleVisibility);
  }, []);

  const scrollToTop = () => {
    window.scrollTo({
      top: 0,
      behavior: "smooth",
    });
  };

  return (
    <>
      {isVisible && (
        <button
          onClick={scrollToTop}
          className="fixed bottom-6 right-6 bg-black text-white p-3 rounded-full shadow-lg hover:bg-gray-800 transition-transform transform hover:scale-110"
        >
          <RiArrowUpSLine size={24} />
        </button>
      )}
    </>
  );
};

export default ScrollToTop;

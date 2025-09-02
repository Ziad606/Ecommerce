import { RiTruckLine, RiHeadphoneLine, RiRefund2Line } from "react-icons/ri";
import f2 from "../assets/f2.jpg"
import f1 from "../assets/f1.png";
import f4 from "../assets/f4.png";
import f3 from "../assets/f3.png";

const Features = () => {
  const featuresData = [
    {
      img: f1,
      alt: "PlayStation 5",
      title: "PlayStation 5",
      description: "Black and White version of the PS5 coming out on sale.",
      className: "md:col-span-2 md:row-span-2",
    },
    {
      img: f2,
      alt: "Women's Collections",
      title: "Women’s Collections",
      description: "Featured woman collections that give you another vibe.",
      className: "md:col-span-2 h-full",
    },
    {
      img: f3,
      alt: "Speakers",
      title: "Speakers",
      description: "Amazon wireless speakers",
    },
    {
      img: f4,
      alt: "Perfume",
      title: "Perfume",
      description: "GUCCI INTENSE OUD EDP",
    },
  ];

  const items = [
    {
      icon: <RiTruckLine size={30} />,
      title: "FREE AND FAST DELIVERY",
      desc: "Free delivery for all orders over $140",
    },
    {
      icon: <RiHeadphoneLine size={30} />,
      title: "24/7 CUSTOMER SERVICE",
      desc: "Friendly 24/7 customer support",
    },
    {
      icon: <RiRefund2Line size={30} />,
      title: "MONEY BACK GUARANTEE",
      desc: "We return money within 30 days",
    },
  ];
  return (
    <div className="mx-auto max-w-[90%] lg:max-w-[1400px] py-8">
      <p className="text-lg text-[rgba(219,68,68,1)] flex items-center">
        <span className="bg-[rgba(219,68,68,1)] w-4 h-10 inline-block mr-2 rounded mb-3"></span>
        Our Features
      </p>
      {/* // cards  */}
      <section className="px-4 md:px-8 lg:px-16 py-12">
        <div className="grid grid-cols-1 md:grid-cols-4 md:grid-rows-2 gap-6 h-[600px]">
          {featuresData.map((feature, index) => (
            <div key={index} className={`relative rounded-lg overflow-hidden ${feature.className}`}>
              <img
                src={feature.img}
                alt={feature.alt}
                className="w-full h-full object-cover"
              />
              <div className="absolute inset-0 bg-black bg-opacity-40"></div>
              <div className="absolute bottom-4 left-4 text-white">
                <h3 className="text-xl font-bold">{feature.title}</h3>
                <p className="text-sm mb-2">{feature.description}</p>
                <button className="underline hover:text-[rgba(219,68,68,1)]">Shop Now</button>
              </div>
            </div>
          ))}
        </div>
      </section>
 {/*  */}
     <section className="max-w-6xl mx-auto px-4 py-12 grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
      {items.map((item, idx) => (
        <div
          key={idx}
          className="flex flex-col items-center gap-4 transform transition-transform duration-300 hover:-translate-y-2"
        >
 <div className="bg-gray-200 rounded-full w-[68px] h-[68px] flex items-center justify-center">
  <div className="w-12 h-12 flex items-center justify-center rounded-full bg-black text-white text-3xl">
    {item.icon}
  </div>
</div>

          <h3 className="font-semibold text-lg">{item.title}</h3>
          <p className="text-gray-500 text-sm">{item.desc}</p>
        </div>
      ))}
    </section>
    </div>
  );
};

export default Features;
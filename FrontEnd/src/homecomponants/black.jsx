import speakers from "../assets/speakers.png"
const Black =() => {
 return (
  /* Frame 600 */
  <section className=" bg-black  text-white py-12  md:px-8 lg:px-16 flex flex-col-reverse md:flex-row items-center justify-between gap-8">

      <div className="flex-1 flex flex-col gap-6">
        <p className="text-green-500 font-semibold">Categories</p>
        <h2 className="text-3xl md:text-4xl lg:text-5xl font-bold">
          Enhance Your Music Experience
        </h2>
         {/*  */}
        <div className="flex gap-4 text-black font-semibold flex-wrap">
          {["23 Hours", "05 Days", "59 Minutes", "35 Seconds"].map((time, index) => (
            <div
              key={index}
              className="bg-white text-black rounded-full px-4 py-2 text-center min-w-[70px] "
            >
              {time}
            </div>
          ))}
        </div>

        <button className="bg-green-500 hover:bg-green-600 text-black font-bold py-3 px-6 rounded-md w-max mt-4">
          Buy Now!
        </button>
      </div>

      {/* Image  */}
      <div className="flex-1 flex justify-center">
        <img
          src={speakers} 
          alt="Music Speaker"
          className="w-full max-w-md object-contain"
        />
      </div>
    </section>
  );
};

 
export default Black;
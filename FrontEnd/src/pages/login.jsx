import SideImage from "../assets/SideImagelogin.png";
import Footer from "../componants/Footer";
const Login = () => {
  return (
    <>
      <main className="flex flex-col lg:flex-row items-center p-4  gap-8 mt-6">
        <img src={SideImage} alt="mobile side image " className="" />
        <form className="flex flex-col gap-6  p-8 rounded-lg  w-full md:w-1/2 lg:w-1/3">
          <h1 className="text-5xl  mb-4">Log in to Exclusive</h1>
          <p>Enter your details below</p>
          {/* floating label input field */}
          <div className="relative w-full flex flex-col gap-6 my-4">
            <input
              type="text"
              id="email"
              placeholder=" "
              className="peer w-full border-b-2 border-gray-300 bg-transparent py-2 text-gray-900 placeholder-transparent focus:border-[rgba(219,68,68,1)] focus:outline-none"
            />
            <label
              htmlFor="email"
              className="absolute left-0 top-2 text-gray-400 transition-all peer-placeholder-shown:top-2 peer-placeholder-shown:text-gray-400 peer-placeholder-shown:text-base peer-focus:top-[-10px] peer-focus:text-sm peer-focus:text-[rgba(219,68,68,1)]"
            >
              Email Address or Phone Number
            </label>
          </div>

          <div className="relative w-full">
            <input
              type="password"
              id="password"
              placeholder=" "
              className="peer w-full border-b-2 border-gray-300 bg-transparent py-2 text-gray-900 placeholder-transparent focus:border-[rgba(219,68,68,1)] focus:outline-none"
            />
            <label
              htmlFor="password"
              className="absolute left-0 top-2 text-gray-400 transition-all peer-placeholder-shown:top-2 peer-placeholder-shown:text-gray-400 peer-placeholder-shown:text-base peer-focus:top-[-10px] peer-focus:text-sm peer-focus:text-[rgba(219,68,68,1)]"
            >
              Password
            </label>
          </div>
          <div className="flex items-center  justify-between flex-col gap-4 md:flex-row">
            <button
              type="submit"
              className=" bg-[rgba(219,68,68,1)] p-3 text-white w-52 rounded-lg"
            >
              {" "}
              Log in
            </button>
            {/* forget password link */}
            <p className="text-[rgba(219,68,68,1)] underline">Forget Your Password </p>
          </div>
        </form>
      </main>
    </>
  );
};

export default Login;

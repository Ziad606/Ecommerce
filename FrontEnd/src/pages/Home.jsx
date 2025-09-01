
import  Hero   from '../componants/hero';
import SalesSection from '../componants/SalesSection';   
import CategorySection from '../componants/CategorySection';

const Home = () => {
  return<>
      <Hero />
      <div className='mx-auto max-w-7xl'>
        {/* sales section  */}
       <SalesSection/>
       <hr />
       {/* category  */}
       <CategorySection/>
       </div>
      
    </>
};
export default Home;

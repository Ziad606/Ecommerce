import { useState, useEffect } from "react";
import { fetchAPI } from "./FetchAPI"; 

// custom hook 
export const useFetch = (url, options = {}) => {
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let isMounted = true;

    const getData = async () => {
      setLoading(true);
      setError(null);
      const result = await fetchAPI(url, options);
      if (isMounted) {
        if (result) setData(result);
        setLoading(false);
      }
    };

    getData();

    return () => {
      isMounted = false; 
    };
  }, [url, options]);

  return { data, loading, error };
};

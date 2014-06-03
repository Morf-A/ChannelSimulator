function psd = psdd(x1,M)
   psd = abs(fft(real(correlation(x1,M)))).^2;
end


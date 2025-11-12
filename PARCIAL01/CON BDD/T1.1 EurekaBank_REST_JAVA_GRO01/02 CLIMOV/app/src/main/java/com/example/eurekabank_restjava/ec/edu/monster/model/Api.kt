package com.example.eurekabank_restjava.ec.edu.monster.model

import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Response
import retrofit2.Retrofit
import retrofit2.converter.scalars.ScalarsConverterFactory
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.*
import java.util.concurrent.TimeUnit

// Emulador = localhost del PC
private const val BASE_URL = "http://192.168.100.170:8080/WSEurekaBank_GRO01/"

interface CoreBancarioApi {

    // ---- GET MOVIMIENTOS (ya te funciona) ----
    @GET("webresources/coreBancario/movimientos/{cuenta}")
    suspend fun movimientos(@Path("cuenta") cuenta: String): List<MovimientoDto>

    // ==========================================================
    // OPCIÓN A)  application/x-www-form-urlencoded  (@FormParam)
    //  Usa esta SI tu backend tiene métodos con @FormParam.
    // ==========================================================
    @FormUrlEncoded
    @POST("webresources/coreBancario/deposito")
    suspend fun depositoForm(
        @Field("cuenta") cuenta: String,
        @Field("importe") importe: Double
    ): Response<Void>

    @FormUrlEncoded
    @POST("webresources/coreBancario/retiro")
    suspend fun retiroForm(
        @Field("cuenta") cuenta: String,
        @Field("importe") importe: Double
    ): Response<Void>

    @FormUrlEncoded
    @POST("webresources/coreBancario/transferencia")
    suspend fun transferenciaForm(
        @Field("origen") origen: String,
        @Field("destino") destino: String,
        @Field("importe") importe: Double
    ): Response<Void>

    // ==========================================================
    // OPCIÓN B)  Path params en la URL  (@Path)
    //  Usa esta SI tu backend expone URLs tipo:
    //   /deposito/{cuenta}/{importe}
    //   /retiro/{cuenta}/{importe}
    //   /transferencia/{origen}/{destino}/{importe}
    // ==========================================================
    @POST("webresources/coreBancario/deposito/{cuenta}/{importe}")
    suspend fun depositoPath(
        @Path("cuenta") cuenta: String,
        @Path("importe") importe: Double
    ): Response<Void>

    @POST("webresources/coreBancario/retiro/{cuenta}/{importe}")
    suspend fun retiroPath(
        @Path("cuenta") cuenta: String,
        @Path("importe") importe: Double
    ): Response<Void>

    @POST("webresources/coreBancario/transferencia/{origen}/{destino}/{importe}")
    suspend fun transferenciaPath(
        @Path("origen") origen: String,
        @Path("destino") destino: String,
        @Path("importe") importe: Double
    ): Response<Void>
}

object Api {
    private val logger = HttpLoggingInterceptor().apply {
        level = HttpLoggingInterceptor.Level.BODY
    }

    private val http = OkHttpClient.Builder()
        .addInterceptor(logger)
        .connectTimeout(20, TimeUnit.SECONDS)
        .readTimeout(30, TimeUnit.SECONDS)
        .build()

    // Scalars primero para poder leer errorBody de texto plano
    val service: CoreBancarioApi = Retrofit.Builder()
        .baseUrl(BASE_URL)
        .client(http)
        .addConverterFactory(ScalarsConverterFactory.create())
        .addConverterFactory(GsonConverterFactory.create())
        .build()
        .create(CoreBancarioApi::class.java)
}
